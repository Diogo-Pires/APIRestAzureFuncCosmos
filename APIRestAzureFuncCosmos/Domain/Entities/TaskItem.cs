using Newtonsoft.Json;

namespace Domain.Entities;

public class TaskItem(string title, string description)
{
    [JsonProperty("id")]
    public string Id { get; private set; } = Guid.NewGuid().ToString();


    [JsonProperty("title")]
    public string Title { get; set; } = title;


    [JsonProperty("description")]
    public string Description { get; set; } = description;


    [JsonProperty("isCompleted")]
    public bool IsCompleted { get; set; } = false;


    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
}