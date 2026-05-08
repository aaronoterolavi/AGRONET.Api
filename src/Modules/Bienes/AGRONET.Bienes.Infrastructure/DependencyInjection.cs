using Microsoft.Extensions.DependencyInjection;
using AGRONET.Bienes.Application.Contracts;
using AGRONET.Bienes.Application.Services;
using AGRONET.Bienes.Infrastructure.Persistence;

namespace AGRONET.Bienes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBienesModule(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IBienesRepository, BienesRepository>();

        // Services
        services.AddScoped<IBienesService, BienesService>();

        return services;
    }
}