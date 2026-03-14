using PersonService.Application.Interfaces.Repositories;
using PersonService.Infra.Data.Common;
using PersonService.Infra.Data.Context;
using PersonService.Infra.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PersonService.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["MongoSettings:ConnectionString"];

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connection));

            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services
            .AddHealthChecks()
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                tags: new[] { "database", "mongodb" })
            .AddCheck("self", () => HealthCheckResult.Healthy());

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRepositoryExecutor, RepositoryExecutor>();
        }
    }
}
