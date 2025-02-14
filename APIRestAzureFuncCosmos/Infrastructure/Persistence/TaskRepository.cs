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

    public async Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<TaskItem> taskList = [];
        using (FeedIterator<TaskItem> setIterator =
                _container.GetItemLinqQueryable<TaskItem>()
                    .ToFeedIterator())
        {
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync(cancellationToken))
                {
                    taskList.Add(item);
                }
            }
        }

        return taskList;
    }

    public async Task<TaskItem?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await _container.ReadItemAsync<TaskItem>(id, new PartitionKey(id), cancellationToken: cancellationToken);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken) =>
        await _container.CreateItemAsync(task, new PartitionKey(task.Id));

    public async Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        var response = await _container.UpsertItemAsync(task, new PartitionKey(task.Id), cancellationToken: cancellationToken);
        return response.StatusCode == HttpStatusCode.OK ? task : null;
    }

    public async Task<bool> DeleteByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.DeleteItemAsync<TaskItem>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}