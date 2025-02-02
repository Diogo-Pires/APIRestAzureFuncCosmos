using Application.Interfaces;
using Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Persistence;

public class TaskRepository : ITaskRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public TaskRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer("DatabaseName", "Tasks");
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        var query = _container.GetItemQueryIterator<TaskItem>("SELECT * FROM Tasks");
        List<TaskItem> results = new();
        while (query.HasMoreResults)
        {
            foreach (var item in await query.ReadNextAsync())
            {
                results.Add(item);
            }
        }
        return results;
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        await _container.CreateItemAsync(task, new PartitionKey(task.Id));
    }
}
