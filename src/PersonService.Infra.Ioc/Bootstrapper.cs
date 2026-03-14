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
    public static class Bootstrapper
    {
        public static void AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            var connection = configuration["MongoSettings:ConnectionString"];

            service.AddSingleton<IMongoClient>(_ => new MongoClient(connection));

            service.AddSingleton<IMongoDbContext, MongoDbContext>();

            service
            .AddHealthChecks()
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                tags: new[] { "database", "mongodb" })
            .AddCheck("self", () => HealthCheckResult.Healthy());

            service.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            service.AddScoped<IRepositoryExecutor, RepositoryExecutor>();
        }
    }
}
