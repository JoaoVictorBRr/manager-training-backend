using Microsoft.Extensions.DependencyInjection;
using Zyntra.Data.Repository;

namespace Zyntra.CrossCutting.DependencyInjection.Repository;

public static class RepositoryDependencyInjection
{
    public static IServiceCollection AddSqlRepositoryDependency(this IServiceCollection services)
    {
        services.Scan(scan => scan
                .FromAssemblyOf<UserRepository>()
                .AddClasses()
                .AsMatchingInterface()
                .WithScopedLifetime());

        return services;
    }
}
