using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Zyntra.CrossCutting.DependencyInjection.Validation.Base;

public static class ValidatorsDependencyInjection
{
    public static IServiceCollection AddValidatorsDependency(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(UserValidator));
        return services;
    }
}
