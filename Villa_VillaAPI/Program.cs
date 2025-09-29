//using Serilog;
using Villa_VillaAPI.Logging;

var builder = WebApplication.CreateBuilder(args);

/*
 ===========================================
         Add services to the container.
 ===========================================
*/

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

//builder.Host.UseSerilog(); // UseSerilog() is an extension method provided by the Serilog.AspNetCore package that configures Serilog as the logging provider for the application.

builder.Services.AddControllers(option => option.ReturnHttpNotAcceptable = true).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddSingleton<ILogging, Logging>();

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
    //app.MapOpenApi();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/openapi/v1.json", "VillaAPI");
    //});
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
