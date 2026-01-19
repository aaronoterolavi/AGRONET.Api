using AGRONET.FichaSalida.Application.Interfaces;
using AGRONET.FichaSalida.Application.Services;
using AGRONET.FichaSalida.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Infrastructure
{
    public static class FichaSalidaModule
    {
        public static IServiceCollection AddAgronetFichaSalida(this IServiceCollection services)
        {
            services.AddScoped<IFichaSalidaRepository, FichaSalidaRepository>();
            services.AddScoped<IFichaSalidaService, FichaSalidaService>();
            return services;
        }
    }
}
