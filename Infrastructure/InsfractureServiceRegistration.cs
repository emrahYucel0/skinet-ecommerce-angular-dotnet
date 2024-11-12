using System;
using System.Reflection.Metadata.Ecma335;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Interfaces;
using StackExchange.Redis;
using Infrastructure.Services;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connString = configuration.GetConnectionString("Redis") 
                ?? throw new Exception("Cannot get redis connection string");
            var configurationOptions = ConfigurationOptions.Parse(connString, true);
            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        services.AddSingleton<ICartService, CartService>();

        return services;
    }
}
