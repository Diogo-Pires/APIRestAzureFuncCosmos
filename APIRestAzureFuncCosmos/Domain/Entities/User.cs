using Newtonsoft.Json;

namespace Domain.Entities;

public class User
{
    [JsonProperty("id")]
    public string Id { get; private set; }


    [JsonProperty("name")]
    public string Name { get; private set; }

    private User() { }

    public User(string name, string email)
    {
        Id = email;
        Name = name;
    }
}