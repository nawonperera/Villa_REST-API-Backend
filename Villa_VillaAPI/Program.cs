using Microsoft.EntityFrameworkCore;
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

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

//builder.Host.UseSerilog(); // UseSerilog() is an extension method provided by the Serilog.AspNetCore package that configures Serilog as the logging provider for the application.

builder.Services.AddControllers(option => { 
    //option.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
 
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
