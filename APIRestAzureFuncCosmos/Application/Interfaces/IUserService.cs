using Application.DTOs;
using FluentResults;

namespace Application.Interfaces;

public interface IUserService
{
    Task<List<UserDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<UserDTO>> CreateAsync(UserDTO createUserDto, CancellationToken cancellationToken);
}