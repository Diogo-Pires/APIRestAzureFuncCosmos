using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Infrastructure.Config;
using Infrastructure.Persistence;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Presentation.Exceptions;
using Presentation.Interfaces;
using Presentation.Validations;
using Shared.Consts;

[assembly: FunctionsStartup(typeof(Presentation.Startup))]
namespace Presentation;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(UtilityConsts.APP_NAME));
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.AddConsoleExporter();
            });
        });

        var configuration = builder.GetContext().Configuration;
        var jaggerSettings = configuration.GetSection("Jagger").Get<JaggerSettings>();

        builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(UtilityConsts.APP_NAME))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(UtilityConsts.APP_NAME)
                    .AddJaegerExporter(opts =>
                    {
                        opts.AgentHost = jaggerSettings.AgentHost;
                        opts.AgentPort = jaggerSettings.AgentPort;
                    })
                )
                .WithMetrics(metrics => metrics
                    .AddMeter(UtilityConsts.APP_NAME)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                );

        var cosmosSettings = configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();

        builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();
        builder.Services.AddSingleton(x => new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key));
        builder.Services.AddSingleton(x => cosmosSettings);
        builder.Services.AddTransient<ITaskRepository, TaskRepository>();
        builder.Services.AddTransient<ITaskService, TaskService>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();

    }
}