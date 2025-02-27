using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class UserMapper
{
    public static User ToEntity(UserDTO dto) =>
        new(dto.Name.Trim(), dto.Id.Trim());

    public static UserDTO ToDTO(User entity) =>
        new(entity.Name, entity.Id);
}