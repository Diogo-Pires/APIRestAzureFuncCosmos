namespace Infrastructure.Config;

public class CosmosDbSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string TaskContainerName { get; set; } = string.Empty;
    public string UserContainerName { get; set; } = string.Empty;
}