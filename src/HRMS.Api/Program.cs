using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using HRMS.Infrastructure.Data;
using HRMS.Api.Middleware;
using HRMS.Api.Filters;
using HRMS.Application.Services;
using HRMS.Infrastructure.Services;
using Serilog;
using AutoMapper;

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

// Phase 2: Register Payroll, Attendance, Leave services (in-memory for Phase 2.1)
builder.Services.AddScoped<IPayrollService, InMemoryPayrollService>();
builder.Services.AddScoped<IAttendanceService, InMemoryAttendanceService>();
builder.Services.AddScoped<ILeaveService, InMemoryLeaveService>();

var app = builder.Build();

// Use error handling middleware before other middleware
app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

