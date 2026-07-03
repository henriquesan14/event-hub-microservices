using Identity.Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssemblyContaining<GenerateAccessTokenCommandValidator>();

        return services;
    }
}
