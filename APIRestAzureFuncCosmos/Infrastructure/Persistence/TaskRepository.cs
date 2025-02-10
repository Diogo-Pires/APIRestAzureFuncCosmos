using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Config;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;

namespace Infrastructure.Persistence;

public class TaskRepository : ITaskRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public TaskRepository(CosmosClient cosmosClient, CosmosDbSettings cosmosDbSettings)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer(cosmosDbSettings.DatabaseName, cosmosDbSettings.ContainerName);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        List<TaskItem> taskList = [];
        using (FeedIterator<TaskItem> setIterator = 
                _container.GetItemLinqQueryable<TaskItem>()
                    .ToFeedIterator())
        {
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                {
                    taskList.Add(item);
                }
            }
        }

        return taskList;
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        try
        {
            return await _container.ReadItemAsync<TaskItem>(id, new PartitionKey(id));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<TaskItem> AddAsync(TaskItem task) =>
        await _container.CreateItemAsync(task, new PartitionKey(task.Id));

    public async Task<TaskItem?> UpdateAsync(TaskItem task)
    {
        var response = await _container.UpsertItemAsync(task, new PartitionKey(task.Id));
        return response.StatusCode == HttpStatusCode.OK ? task : null;
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        try
        {
            var response = await _container.DeleteItemAsync<TaskItem>(id, new PartitionKey(id));
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}   