using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using HRMS.Infrastructure.Data;
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
builder.Services.AddControllers();
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

// AutoMapper (MediatR deferred to Phase 2 when needed)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register ICurrentUser implementation in HRMS.Api (populated per request)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HRMS.Application.Common.ICurrentUser, HRMS.Api.Identity.CurrentUser>();

var app = builder.Build();

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

