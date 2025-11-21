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

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; // If the client does not specify an API version, use the default version
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0); // Set the default API version to 1.0
    options.ReportApiVersions = true; // Report the supported API versions in the response headers
});

builder.Services.AddVersionedApiExplorer(options =>
{
    // Format of API version names in Swagger and other tools
    // "'v'VVV" means:
    //   - Start with "v"
    //   - Followed by a 3-digit version number
    // Examples:
    //   v1  -> v001
    //   v2  -> v002
    //   v10 -> v010
    // This helps keep Swagger docs consistent for multiple API versions.
    options.GroupNameFormat = "'v'VVV";

    // Replace the version placeholder in route URLs with the actual version number
    // Example:
    //   Route:      "api/v{version}/users"
    //   Version:    1
    //   Final URL:  "api/v1/users"
    options.SubstituteApiVersionInUrl = true;
});


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
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Crystal Villa V1",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Dotnetmastery",
            Url = new Uri("https://dotnetmastery.com")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Crystal Villa v2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Dotnetmastery",
            Url = new Uri("https://dotnetmastery.com")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VillaV1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "VillaV2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
