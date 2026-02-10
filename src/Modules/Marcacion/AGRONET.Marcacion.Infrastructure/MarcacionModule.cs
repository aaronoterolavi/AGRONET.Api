using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AGRONET.Marcacion.Application;

namespace AGRONET.Marcacion.Infrastructure;

public static class MarcacionModule
{
    public static IServiceCollection AddAgronetMarcacion(this IServiceCollection services, IConfiguration config)
    {
        services.AddAgronetMarcacionApplication();
        services.AddAgronetMarcacionInfrastructure(config);
        return services;
    }
}
