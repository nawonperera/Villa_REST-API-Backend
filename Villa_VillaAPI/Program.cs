using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Villa_VillaAPI;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Repository;
using Villa_VillaAPI.Repository.IRepository;


var builder = WebApplication.CreateBuilder(args);

/*
 ===========================================
         Add services to the container.
 ===========================================
*/

builder.Services.AddAutoMapper(typeof(MappingConfig)); // AutoMapper is a library that helps to map objects of one type to another type. It is used to map DTOs (Data Transfer Objects) to Entities and vice versa.)
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog(); // UseSerilog() is an extension method provided by the Serilog.AspNetCore package that configures Serilog as the logging provider for the application.

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(x =>    // Add authentication services to the DI container
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme for [Authorize] checks
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default challenge scheme (used when authentication fails)
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false; // Do not require HTTPS (useful for local development; should be true in production)
        x.SaveToken = true;  // Save the token in the AuthenticationProperties after a successful authorization
        x.TokenValidationParameters = new TokenValidationParameters // Set the parameters for validating incoming JWT tokens
        {
            ValidateIssuerSigningKey = true, // Validate that the token's signature matches our secret key
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key)), // Validate that the token's signature matches our secret key
            ValidateIssuer = false, // Do not validate the issuer of the token
            ValidateAudience = false // Do not validate the audience of the token
        };
    });

builder.Services.AddControllers(option =>
{
    //option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

/*
 ===========================================
     Configure the HTTP request pipeline.
 ===========================================
*/

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
