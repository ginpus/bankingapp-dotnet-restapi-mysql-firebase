using Domain.Client;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Domain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            return services
                .AddClients()
                .AddServices();
        }

        private static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddHttpClient<IAuthClient, AuthClient>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // IHttpContextAccessor is not wired up by default. Required for UserResolverService

            services
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IAccountService, AccountService>()
                .AddTransient<IUserResolverService, UserResolverService>();

            return services;
        }

    }

}
