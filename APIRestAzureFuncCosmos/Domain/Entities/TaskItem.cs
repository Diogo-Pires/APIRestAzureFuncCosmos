namespace Domain.Entities;

public class TaskItem(string title, string description)
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public bool IsCompleted { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
}