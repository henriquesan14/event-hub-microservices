using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Api.Extensions;

public static class SerializationExtensions
{
    public static IServiceCollection AddJsonSerializationConfig(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        return services;
    }
}
