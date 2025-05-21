using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.MonitoringService.Data;
using PatientRecoverySystem.MonitoringService.BackgroundServices;
using PatientRecovery.MonitoringService.Services;
using PatientRecoverySystem.MonitoringService.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IVitalSignsMonitoringService, VitalSignsMonitoringService>();

// Background Service
builder.Services.AddHostedService<VitalSignsMonitoringBackgroundService>();

// RabbitMQ
builder.Services.AddSingleton<IRabbitMQService>(sp => 
    new RabbitMQService(builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost"));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();