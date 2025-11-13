using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Model;
using Villa_VillaAPI.Model.Dto;
using Villa_VillaAPI.Repository.IRepository;

namespace Villa_VillaAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private string secretKey;

    public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }

    public bool IsUniqueUser(string username)
    {
        var user = _dbContext.LocalUsers.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
    {
        var user = _dbContext.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
        && u.Password == loginRequestDTO.Password);

        if (user == null)
        {
            return new()
            {
                Token = "",
                User = null
            };
        }

        // if user is found generate JWT Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                // Add a claim for the user's unique ID
                // ClaimTypes.Name is a standard claim type used for identifying the user
                // eg : This user's Name is their Id
                new Claim(ClaimTypes.Name, user.Id.ToString()),

                //This user's Role is whatever role is in user.Role
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); //Token not Serialized yet
        LoginResponseDto loginResponseDTO = new()
        {
            Token = tokenHandler.WriteToken(token), //Token Serializes here by writetoken
            User = user
        };
        return loginResponseDTO;
    }

    public async Task<LocalUser> Register(RegistrationRequestDto registerationRequestDTO)
    {
        LocalUser user = new()
        {
            UserName = registerationRequestDTO.UserName,
            Name = registerationRequestDTO.Name,
            Password = registerationRequestDTO.Password,
            Role = registerationRequestDTO.Role
        };
        _dbContext.LocalUsers.Add(user);
        await _dbContext.SaveChangesAsync();
        user.Password = "";
        return user;

    }
}
