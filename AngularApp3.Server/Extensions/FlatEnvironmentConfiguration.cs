namespace AngularApp3.Server.Extensions;

public static class FlatEnvironmentConfiguration
{
    public static void ApplyFlatEnvironment(this ConfigurationManager configuration)
    {
        CopyUnlessCanonical(configuration, "Admin:Email", "ADMIN_EMAIL");
        CopyUnlessCanonical(configuration, "Admin:Password", "ADMIN_PASSWORD");
        CopyUnlessCanonical(configuration, "Jwt:Issuer", "JWT_ISSUER");
        CopyUnlessCanonical(configuration, "Jwt:Audience", "JWT_AUDIENCE");
        CopyUnlessCanonical(configuration, "Jwt:Key", "JWT_KEY");
        CopyUnlessCanonical(configuration, "Antiforgery:SameSite", "ANTIFORGERY_SAMESITE");
        CopyUnlessCanonical(configuration, "Supabase:Url", "SUPABASE_URL");
        CopyUnlessCanonical(configuration, "Supabase:PublicUrl", "SUPABASE_PUBLIC_URL");
        CopyUnlessCanonical(configuration, "Supabase:ServiceRoleKey", "SUPABASE_SERVICE_ROLE_KEY");
        CopyUnlessCanonical(configuration, "Supabase:Bucket", "SUPABASE_BUCKET");
        CopyUnlessCanonical(configuration, "Cors:AllowedOrigins", "CORS_ALLOWED_ORIGINS");
        CopyUnlessCanonical(configuration, "Redis:ConnectionString", "REDIS_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")))
        {
            return;
        }

        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
        if (string.IsNullOrWhiteSpace(port))
        {
            port = "5432";
        }

        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty;
        var ssl = host is "localhost" or "db" ? "Disable" : "Require";
        configuration["ConnectionStrings:DefaultConnection"] =
            $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={ssl};Trust Server Certificate=true";
    }

    private static void CopyUnlessCanonical(ConfigurationManager configuration, string target, string flatName)
    {
        var canonical = Environment.GetEnvironmentVariable(target.Replace(":", "__"));
        if (!string.IsNullOrWhiteSpace(canonical))
        {
            return;
        }

        var flat = Environment.GetEnvironmentVariable(flatName);
        if (string.IsNullOrWhiteSpace(flat))
        {
            return;
        }

        configuration[target] = flat;
    }
}
