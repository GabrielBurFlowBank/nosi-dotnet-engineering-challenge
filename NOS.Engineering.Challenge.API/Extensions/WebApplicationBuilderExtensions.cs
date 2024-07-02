using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Context;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder webApplicationBuilder)
    {
        var serviceCollection = webApplicationBuilder.Services;

        serviceCollection.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = null;
        });
        serviceCollection.AddControllers();
        serviceCollection
            .AddEndpointsApiExplorer();

        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nos Challenge Api", Version = "v1" });
        });

        serviceCollection.AddMemoryCache();

        serviceCollection
            .RegisterCacheService()
            //.RegisterFastDatabase()
            .RegisterSlowDatabase()
            .RegisterContentsManager();

        return webApplicationBuilder;
    }

    private static IServiceCollection RegisterFastDatabase(this IServiceCollection services)
    {
        //services.AddDbContext<EFSQLServerContext>(
        //    options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;"));

        services.AddScoped<IDatabase<Content, ContentDto>, FastDatabase<Content, ContentDto>>();
        services.AddScoped<IMapper<Content, ContentDto>, ContentMapper>();
        
        return services;
    }

    private static IServiceCollection RegisterSlowDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDatabase<Content, ContentDto>,SlowDatabase<Content, ContentDto>>();
        services.AddSingleton<IMapper<Content, ContentDto>, ContentMapper>();
        services.AddSingleton<IMockData<Content>, MockData>();

        return services;
    }
    
    private static IServiceCollection RegisterContentsManager(this IServiceCollection services)
    {
        services.AddScoped<IContentsManager, ContentsManager>();

        return services;
    }

    private static IServiceCollection RegisterCacheService(this IServiceCollection services)
    {
        services.AddScoped<ICacheService, InMemoryCacheService>();

        return services;
    }
    
    
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder
            .WebHost
            .ConfigureLogging(logging => { /*logging.ClearProviders(); */ logging.AddConsole(); });

        return webApplicationBuilder;
    }
}