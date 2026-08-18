using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using HRMS.Infrastructure.Data;
using HRMS.Infrastructure.Configuration;
using HRMS.Api.Middleware;
using HRMS.Api.Filters;
using HRMS.Application.Services;
using HRMS.Infrastructure.Services;
using Serilog;
using AutoMapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/hrms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Basic services for Phase 1
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(local);Database=HRMS;Integrated Security=true;TrustServerCertificate=true;";
builder.Services.AddDbContext<HrmsDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.MigrationsAssembly("HRMS.Infrastructure")));

// Health checks
builder.Services.AddHealthChecks();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// FluentValidation
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// Register ICurrentUser implementation in HRMS.Api (populated per request)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HRMS.Application.Common.ICurrentUser, HRMS.Api.Identity.CurrentUser>();

// Phase 2.1: Register Payroll, Attendance, Leave services (in-memory)
builder.Services.AddScoped<IPayrollService, InMemoryPayrollService>();
builder.Services.AddScoped<IAttendanceService, InMemoryAttendanceService>();
builder.Services.AddScoped<ILeaveService, InMemoryLeaveService>();
builder.Services.AddScoped<IEmployeeService, InMemoryEmployeeService>();
// Phase 2.2: Register Authentication services (JWT + Refresh tokens)
var jwtConfig = new JwtConfiguration();
builder.Configuration.GetSection("Jwt").Bind(jwtConfig);
builder.Services.AddSingleton(jwtConfig);
builder.Services.AddScoped<IAuthService, InMemoryAuthService>();

// Phase 2.2: Configure JWT authentication scheme
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // No clock skew tolerance
        };

        // Log JWT bearer token errors
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning($"JWT authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Log.Warning("JWT challenge: missing or invalid token");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Use error handling middleware before other middleware
app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Phase 2.2: Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

