using Admission.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssemblyContaining<CheckInTicketCommandValidator>();
        return services;
    }
}
