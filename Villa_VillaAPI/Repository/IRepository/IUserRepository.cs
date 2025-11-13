using Villa_VillaAPI.Model;
using Villa_VillaAPI.Model.Dto;

namespace Villa_VillaAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);
    Task<LocalUser> Register(RegistrationRequestDto registerationRequestDTO);
}
