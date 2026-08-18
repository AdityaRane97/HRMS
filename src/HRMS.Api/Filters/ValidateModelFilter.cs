using FluentValidation;
using HRMS.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMS.Api.Filters;

/// <summary>
/// Validation filter attribute for automatic DTO validation.
/// Applies FluentValidation to action parameters.
/// </summary>
public class ValidateModelFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidateModelFilter> _logger;

    public ValidateModelFilter(IServiceProvider serviceProvider, ILogger<ValidateModelFilter> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var argument in context.ActionArguments)
        {
            if (argument.Value == null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.Value.GetType());
            var validator = _serviceProvider.GetService(validatorType);

            if (validator == null)
                continue;

            try
            {
                var validationMethod = validator.GetType()
                    .GetMethod("ValidateAsync", new[] { argument.Value.GetType(), typeof(CancellationToken) });

                if (validationMethod != null)
                {
                    var validationTask = (Task)validationMethod.Invoke(validator, new[] { argument.Value, CancellationToken.None })!;
                    await validationTask;

                    // Get ValidationResult from the async result
                    var resultProperty = validationTask.GetType().GetProperty("Result");
                    var result = (FluentValidation.Results.ValidationResult?)resultProperty?.GetValue(validationTask);

                    if (result != null && !result.IsValid)
                    {
                        foreach (var error in result.Errors)
                        {
                            if (!errors.ContainsKey(error.PropertyName))
                                errors[error.PropertyName] = new string[] { };

                            errors[error.PropertyName] = errors[error.PropertyName].Append(error.ErrorMessage).ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during validation of {ArgumentName}", argument.Key);
            }
        }

        if (errors.Count > 0)
        {
            _logger.LogWarning("Validation failed for request: {Errors}", string.Join("; ", errors.SelectMany(e => e.Value)));
            throw new Application.Exceptions.ValidationException(errors);
        }

        await next();
    }
}
