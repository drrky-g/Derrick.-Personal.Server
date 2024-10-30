using Derrick.Personal.Repository.Interfaces;
using Derrick.Personal.Repository.Services;
using Microsoft.AspNetCore.Identity.Data;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Derrick.Personal.Server;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IHostingEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
        
        var config = builder.Build();

        if (env.IsDevelopment())
        {
            //dev only startup
        }
        
        builder.AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //inject services
        services.AddSingleton<IConfiguration>();

        services.AddSingleton<ITokenGenerator, AdminTokenGeneratorService>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            ArgumentNullException.ThrowIfNull(config);
            var adminTokenSetting = config.GetValue<string>("AdminToken");
            ArgumentNullException.ThrowIfNull(adminTokenSetting);
            return new AdminTokenGeneratorService(adminTokenSetting);
        });
        
        
        // Add services to the container.
        services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

    }
}