using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using EventsApplication.Validators;

namespace EventsApplication;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssemblyContaining<CreateEventCommandValidator>();

        return services;
    }
}
