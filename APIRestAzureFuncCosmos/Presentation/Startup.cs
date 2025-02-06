using Application.Services;
using Application.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Infrastructure.Persistence;
using Infrastructure.Config;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(MyFunctionApp.Startup))]
namespace MyFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Obtenção das configurações do CosmosDB
            var cosmosSettings = builder.GetContext().Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();

            // Registrando dependências
            builder.Services.AddSingleton(x => new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key));
            builder.Services.AddTransient<ITaskRepository, TaskRepository>();
            builder.Services.AddTransient<ITaskService, TaskService>();
        }
    }
}