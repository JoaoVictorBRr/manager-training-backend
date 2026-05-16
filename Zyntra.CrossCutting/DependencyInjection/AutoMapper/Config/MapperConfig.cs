using Microsoft.Extensions.DependencyInjection;

namespace Zyntra.CrossCutting.DependencyInjection.AutoMapper.Config;

public static class MapperConfig
{
    public static IServiceCollection AddMapperConfiguration(this IServiceCollection services)
    {
        // Criação do service provider para capturar o ILoggerFactory
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, typeof(MappingProfile).Assembly);

        return services;
    }
}
