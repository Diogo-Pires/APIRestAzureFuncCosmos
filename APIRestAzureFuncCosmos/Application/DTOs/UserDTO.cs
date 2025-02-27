namespace Application.DTOs;

public record UserDTO
{
    public string Id { get; init; }
    public string Name { get; init; }

    public UserDTO(string name, string email)
    {
        Id = email;
        Name = name;
    }
}