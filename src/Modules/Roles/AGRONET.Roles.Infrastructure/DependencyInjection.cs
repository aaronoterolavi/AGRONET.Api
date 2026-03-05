using AGRONET.Roles.Application.Interfaces;
using AGRONET.Roles.Application.Services;
using AGRONET.Roles.Infrastructure.Data;
using AGRONET.Roles.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Roles.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRolesModule(this IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
            return services;
        }
    }
}
