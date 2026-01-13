using AGRONET.Auth.Application.Interfaces;
using AGRONET.Auth.Application.Services;
using AGRONET.Auth.Infrastructure.Configuration;
using AGRONET.Auth.Infrastructure.Data;
using AGRONET.Auth.Infrastructure.Data.Repositories;
using AGRONET.Auth.Infrastructure.DirectoryServices;
using AGRONET.Auth.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Auth.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAgronetAuth(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtOptions>(config.GetSection("Jwt"));
            services.Configure<LdapOptions>(config.GetSection("Ldap"));

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuditService, AuditService>();

            services.AddScoped<IAdAuthService, LdapAdAuthService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
