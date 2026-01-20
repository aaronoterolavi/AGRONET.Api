using AGRONET.FichaSalida.Application.Configuration;
using AGRONET.FichaSalida.Application.Interfaces;
using AGRONET.FichaSalida.Application.Services;
using AGRONET.FichaSalida.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgronetFichaSalida(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<StorageOptions>(config.GetSection("Storage"));
            services.Configure<UploadsOptions>(config.GetSection("Uploads"));

            services.AddScoped<IFichaSalidaRepository, FichaSalidaRepository>();
            services.AddScoped<IFichaSalidaAdjuntoRepository, FichaSalidaAdjuntoRepository>();
            services.AddScoped<IFichaSalidaService, FichaSalidaService>();
          

            return services;
        }
    }
}
