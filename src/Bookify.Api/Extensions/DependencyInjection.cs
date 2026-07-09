using Asp.Versioning;
using Bookify.Api.OpenApi;
using Web.Api.Infrastructure;

namespace Web.Api.Extensions;

internal static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
            options.CustomSchemaIds(type => type.FullName?.Replace("+", ".")));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }
}
