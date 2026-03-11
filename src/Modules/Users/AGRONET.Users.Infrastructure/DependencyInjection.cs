using AGRONET.Users.Application.Interfaces;
using AGRONET.Users.Application.Services;
using AGRONET.Users.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AGRONET.Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersModule(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}