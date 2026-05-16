using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zyntra.Service.Service;

namespace Zyntra.CrossCutting.DependencyInjection.Service;

public static class ServiceDependencyInjection
{
    public static IServiceCollection AddServiceDependency(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<HttpClient>();

        services.Scan(scan => scan
                .FromAssemblyOf<UserService>()
                .AddClasses()
                .AsMatchingInterface()
                .WithScopedLifetime());

        return services;
    }
}
