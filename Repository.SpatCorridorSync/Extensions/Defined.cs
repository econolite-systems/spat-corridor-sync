using Microsoft.Extensions.DependencyInjection;

namespace Repository.SpatCorridorSync.Extensions;

public static class Defined
{
    public static IServiceCollection AddSpatIntersectionRepo(this IServiceCollection services)
    {
        services.AddScoped<ISpatIntersectionRepository, SpatIntersectionRepository>();

        return services;
    }
    
    public static IServiceCollection AddSpatCorridorRepo(this IServiceCollection services)
    {
        services.AddScoped<ISpatCorridorRepository, SpatCorridorRepository>();

        return services;
    }
}