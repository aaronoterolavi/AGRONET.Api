using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AGRONET.Marcacion.Application.Abstractions;
using AGRONET.Marcacion.Infrastructure.Persistence;

namespace AGRONET.Marcacion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAgronetMarcacionInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // config no es obligatorio aquí, pero lo dejo por consistencia con tus otros módulos
        services.AddScoped<IMarcacionRepository, MarcacionRepository>();
        return services;
    }
}
