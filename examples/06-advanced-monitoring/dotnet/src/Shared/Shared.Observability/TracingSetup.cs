using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Observability;

public static class TracingSetup
{
    public static ActivitySource ActivitySource { get; private set; } = null!;

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        string serviceName,
        string? jaegerEndpoint = null)
    {
        ActivitySource = new ActivitySource(serviceName);

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(serviceName))
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("Confluent.Kafka.*");

                if (!string.IsNullOrEmpty(jaegerEndpoint))
                {
                    builder.AddJaegerExporter(options =>
                    {
                        options.AgentHost = jaegerEndpoint;
                        options.AgentPort = 6831;
                    });
                }
                else
                {
                    builder.AddConsoleExporter();
                }
            });

        return services;
    }
}
