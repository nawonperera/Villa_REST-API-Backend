using Villa_Web.Models.Dto;

namespace Villa_Web.Model.Dto;

public class LoginResponseDto
{
    public UserDto User { get; set; }
    public string Token { get; set; }
}
