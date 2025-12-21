using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Shared.Observability;
using NavCalculator;

var builder = WebApplication.CreateBuilder(args);

var instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "nav-calculator";
builder.Services.AddObservability(
    instanceId,
    builder.Configuration["Jaeger:AgentHost"]);

builder.Services.AddSingleton(sp => 
    new ServiceMetrics(instanceId));

builder.Services.AddHostedService<NavCalculatorConsumer>();

var app = builder.Build();

app.MapMetrics();

app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy",
    service = instanceId,
    timestamp = DateTime.UtcNow 
}));

app.UseMiddleware<CorrelationMiddleware>();

app.Run();
