using Application.Services;
using Application.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Infrastructure.Persistence;
using Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Presentation.Validations;
using FluentValidation;

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