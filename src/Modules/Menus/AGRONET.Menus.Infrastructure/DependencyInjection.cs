using AGRONET.Menus.Application.Interfaces;
using AGRONET.Menus.Application.Services;
using AGRONET.Menus.Infrastructure.Data;
using AGRONET.Menus.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Menus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMenusModule(this IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuService, MenuService>();

            services.AddScoped<IMenuAdminRepository, MenuAdminRepository>();
            services.AddScoped<IMenuAdminService, MenuAdminService>();

            return services;
        }
    }
}