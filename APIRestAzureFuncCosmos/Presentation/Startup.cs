using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Infrastructure.Config;
using Infrastructure.Persistence;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Validations;

[assembly: FunctionsStartup(typeof(Presentation.Startup))]
namespace Presentation;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var cosmosSettings = builder.GetContext().Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();

        builder.Services.AddSingleton(x => new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key));
        builder.Services.AddSingleton(x => cosmosSettings);
        builder.Services.AddTransient<ITaskRepository, TaskRepository>();
        builder.Services.AddTransient<ITaskService, TaskService>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();

    }
}