using AGRONET.Marcacion.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Marcacion.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAgronetMarcacionApplication(this IServiceCollection services)
    {
        services.AddScoped<MarcacionService>();
        return services;
    }
}
