using AGRONET.Boletas.Application.Interfaces;
using AGRONET.Boletas.Application.Services;
using AGRONET.Boletas.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Boletas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBoletasModule(this IServiceCollection services)
    {
        services.AddScoped<IBoletasRepository, BoletasRepository>();
        services.AddScoped<IBoletasService, BoletasService>();

        return services;
    }
}