using Newtonsoft.Json;

namespace Domain.Entities;

public class User(string name, string email)
{
    [JsonProperty("id")]
    public string Id { get; private set; } = email;


    [JsonProperty("name")]
    public string Name { get; private set; } = name;
}