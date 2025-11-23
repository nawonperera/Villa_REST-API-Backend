using Villa_VillaAPI.Model.Dto;

namespace Villa_VillaAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);
    Task<UserDto> Register(RegistrationRequestDto registerationRequestDTO);
}
