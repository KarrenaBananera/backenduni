using AngularApp3.Server.Configuration;

namespace AngularApp3.Server.Extensions;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = GetAllowedOrigins(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    return;
                }

                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        var section = configuration.GetSection($"{CorsOptions.SectionName}:AllowedOrigins");
        var configured = section.Get<string[]>();

        if (configured is not { Length: > 0 })
        {
            var raw = section.Value;
            configured = string.IsNullOrWhiteSpace(raw)
                ? []
                : raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return configured
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Where(origin => origin.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
