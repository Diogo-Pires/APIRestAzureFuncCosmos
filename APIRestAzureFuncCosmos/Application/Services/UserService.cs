using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using FluentResults;
using FluentValidation;

namespace Application.Services;

public class UserService(IUserRepository userRepository,
                         IValidator<UserDTO> createValidator,
                         IHybridCacheService hybridCacheService) : BaseHybridCacheService, IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHybridCacheService _cacheService = hybridCacheService;
    private readonly IValidator<UserDTO> _createValidator = createValidator;

    protected override string CacheKey { get => "user:"; }

    public async Task<List<UserDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cachekey = $"{CacheKey}{BASE_CACHEKEY_ALL}";
        return await _cacheService
            .GetOrSetAsync(cachekey, async () =>
                (await _userRepository.GetAllAsync(cancellationToken))
                        .Select(UserMapper.ToDTO)
                        .ToList()
            ) ?? [];
    }

    public async Task<UserDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.Trim();
        var cachekey = $"{CacheKey}{email}";

        var user = await _cacheService
            .GetOrSetAsync(cachekey, async () =>
                await _userRepository.GetByEmailAsync(email, cancellationToken)
            );

        if (user == null)
        {
            return null;
        }

        return UserMapper.ToDTO(user);
    }

    public async Task<Result<UserDTO>> CreateAsync(UserDTO createUserDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(createUserDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }

        var userEntity = UserMapper.ToEntity(createUserDto);
        var createdUser = await _userRepository.AddAsync(userEntity, cancellationToken);
        
        await ClearAllRequestFromCacheAsync(_cacheService);

        return Result.Ok(UserMapper.ToDTO(createdUser));
    }
}