using Derrick.Personal.Repository.Interfaces;
using Derrick.Personal.Repository.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity.Data;

namespace Derrick.Personal.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        
        IConfiguration config = configBuilder.Build();
        
        builder.Configuration.AddConfiguration(config);

        
        builder.Services.AddSingleton<IAdminTokenGenerator, AdminTokenGeneratorService>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            ArgumentNullException.ThrowIfNull(config);
            var adminTokenSetting = config.GetValue<string>("AdminToken");
            ArgumentNullException.ThrowIfNull(adminTokenSetting);
            return new AdminTokenGeneratorService(adminTokenSetting);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };




        app.MapPost("/login", (LoginRequest req, IAdminTokenGenerator generator) =>
            {
                //add password validation logic
                return new
                {
                    access_token = generator.GenerateToken(req.Email)
                };
            })
            .WithName("Login")
            .WithOpenApi();
        
        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        {
                            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            TemperatureC = Random.Shared.Next(-20, 55),
                            Summary = summaries[Random.Shared.Next(summaries.Length)]
                        })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();



        app.Run();
    }
}