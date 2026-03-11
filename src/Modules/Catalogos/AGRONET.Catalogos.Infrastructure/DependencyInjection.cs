using AGRONET.Catalogos.Application.Interfaces;
using AGRONET.Catalogos.Application.Services;
using AGRONET.Catalogos.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Catalogos.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogosModule(this IServiceCollection services)
        {
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<ICatalogosService, CatalogosService>();

            return services;
        }
    }
}