using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Persistence.Client;
using Persistence.Models;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            return services
                .AddSqlClient(configuration)
                .AddRepositories();
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IAccountRepository, AccountRepository>();
        }

        public static IServiceCollection AddSqlClient(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ConnectionStrings")["SqlConnectionString"];

            return services.AddTransient<ISqlClient>(_ => new SqlClient(connectionString));
        }
    }
}