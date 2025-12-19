using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Shared.Observability;
using PricingService.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger configuration - requires Swashbuckle.AspNetCore package
// builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddObservability(
    "pricing-service",
    builder.Configuration["Jaeger:AgentHost"]);

builder.Services.AddSingleton(sp => 
    new ServiceMetrics("pricing-service"));

builder.Services.AddSingleton<PricingEventProducer>();

var app = builder.Build();

// Swagger UI - requires Swashbuckle.AspNetCore package
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseCors();
app.MapMetrics();

app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy",
    service = "pricing-service",
    timestamp = DateTime.UtcNow 
}));

app.UseMiddleware<CorrelationMiddleware>();
app.MapControllers();

app.Run();
