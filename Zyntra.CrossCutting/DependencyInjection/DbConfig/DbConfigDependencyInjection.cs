using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zyntra.Data.Context;
namespace Zyntra.CrossCutting.DependencyInjection.DbConfig;

public static class DbConfigDependencyInjection
{
    public static IServiceCollection AddMySqlDependency(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ZyntraContext>(options =>
        {
            var vConnectionString = configuration["ConnectionString"];
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));
            options.UseMySql(vConnectionString, serverVersion).LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        });

        return services;
    }

    public static IServiceCollection UpdateDatabase(this IServiceCollection services, IApplicationBuilder app)
    {
        var seconds = 60;
        var minutes = 20;
        var commandTimeout = seconds * minutes;

        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<ZyntraContext>();
        context.Database.SetCommandTimeout(commandTimeout);

        if (context.Database.GetPendingMigrations().Any())
            context.Database.Migrate();
        context.Database.SetCommandTimeout(null);

        return services;
    }
}
