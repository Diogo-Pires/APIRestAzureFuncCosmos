using Application.Interfaces;
using Application.Services;
using Infrastructure.Config;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var cosmosSettings = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();

builder.Services.AddSingleton(x => new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key));
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
