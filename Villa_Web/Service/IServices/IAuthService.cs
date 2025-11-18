using Villa_Web.Model.Dto;

namespace Villa_Web.Service.IServices;

public interface IAuthService
{
    Task<T> LoginAsync<T>(LoginRequestDto objToCreate);
    Task<T> RegisterAsync<T>(RegistrationRequestDto objToCreate);
}
