using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Shared.Observability;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddObservability(
    "notification-service",
    builder.Configuration["Jaeger:AgentHost"]);

builder.Services.AddSingleton(sp => 
    new ServiceMetrics("notification-service"));

builder.Services.AddHostedService<NotificationConsumer>();

var app = builder.Build();

app.MapMetrics();

app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy",
    service = "notification-service",
    timestamp = DateTime.UtcNow 
}));

app.UseMiddleware<CorrelationMiddleware>();

app.Run();
