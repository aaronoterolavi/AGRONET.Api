using AGRONET.Notificaciones.Application.Interfaces;
using AGRONET.Notificaciones.Application.Services;
using AGRONET.Notificaciones.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Notificaciones.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificacionesModule(this IServiceCollection services)
    {
        services.AddScoped<INotificacionesRepository, NotificacionesRepository>();
        services.AddScoped<INotificacionesService, NotificacionesService>();
        return services;
    }
}
