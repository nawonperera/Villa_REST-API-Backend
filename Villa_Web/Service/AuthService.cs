using Villa_Utility;
using Villa_Web.Model.Dto;
using Villa_Web.Models;
using Villa_Web.Service.IServices;

namespace Villa_Web.Service;

public class AuthService : BaseService, IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string villaUrl;
    public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
    }

    public Task<T> LoginAsync<T>(LoginRequestDto obj)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = villaUrl + "/api/v1/UsersAuth/login"
        });
    }

    public Task<T> RegisterAsync<T>(RegistrationRequestDto obj)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = villaUrl + "/api/v1/UsersAuth/register"
        });
    }
}
