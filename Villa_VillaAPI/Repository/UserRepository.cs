using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager; // Handles all user-related operations in ASP.NET Core Identity, such as creating users, updating them, deleting them, and managing passwords/roles.
    private readonly RoleManager<IdentityRole> _roleManager; // Manages roles in ASP.NET Core Identity, allowing you to create, update, delete, and manage roles for users.
    private string secretKey;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration,
        UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public bool IsUniqueUser(string username)
    {
        var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
    {
        var user = _dbContext.ApplicationUsers
            .FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (user == null || isValid == false)
        {
            return new()
            {
                Token = "",
                User = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
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
                //new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName.ToString()),

                //This user's Role is whatever role is in user.Role
                //new Claim(ClaimTypes.Role, user.Role)
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); //Token not Serialized yet
        LoginResponseDto loginResponseDTO = new()
        {
            Token = tokenHandler.WriteToken(token), //Token Serializes here by writetoken
            User = _mapper.Map<UserDto>(user),
            //Role = roles.FirstOrDefault()
        };
        return loginResponseDTO;
    }

    public async Task<UserDto> Register(RegistrationRequestDto registerationRequestDTO)
    {
        ApplicationUser user = new()
        {
            UserName = registerationRequestDTO.UserName,
            Email = registerationRequestDTO.UserName,
            NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
            Name = registerationRequestDTO.Name,
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())// GetAwaiter().GetResult() forces the async method to run synchronously.
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin")); // Creating "admin" role if it doesn't exist
                    await _roleManager.CreateAsync(new IdentityRole("customer"));
                }
                await _userManager.AddToRoleAsync(user, "admin"); // Assigning "admin" role to the newly registered user
                var userToReturn = _dbContext.ApplicationUsers
                    .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                return new UserDto()
                {
                    ID = userToReturn.Id,
                    Name = userToReturn.Name,
                    UserName = userToReturn.UserName,
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //_dbContext.LocalUsers.Add(user);
        //await _dbContext.SaveChangesAsync();
        //user.Password = "";
        //return user;

        return new UserDto(); //return an empty user dto if registration fails

    }
}
