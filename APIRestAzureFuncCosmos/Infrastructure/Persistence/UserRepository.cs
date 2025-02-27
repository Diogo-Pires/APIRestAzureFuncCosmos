using Application.Interfaces;
using Infrastructure.Config;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using System.Net;

namespace Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public UserRepository(CosmosClient cosmosClient, CosmosDbSettings cosmosDbSettings)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer(cosmosDbSettings.DatabaseName, cosmosDbSettings.UserContainerName);
    }

    public async Task<List<Domain.Entities.User>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<Domain.Entities.User> userList = [];
        using (FeedIterator<Domain.Entities.User> setIterator =
                _container.GetItemLinqQueryable<Domain.Entities.User>()
                    .ToFeedIterator())
        {
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync(cancellationToken))
                {
                    userList.Add(item);
                }
            }
        }

        return userList;
    }

    public async Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            return await _container.ReadItemAsync<Domain.Entities.User>(email, new PartitionKey(email), cancellationToken: cancellationToken);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Domain.Entities.User> AddAsync(Domain.Entities.User user, CancellationToken cancellationToken) =>
        await _container.CreateItemAsync(user, new PartitionKey(user.Id), cancellationToken: cancellationToken);
}