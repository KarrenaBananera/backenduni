using AngularApp3.Server.Configuration;
using StackExchange.Redis;

namespace AngularApp3.Server.Extensions;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();
            var connectionString = string.IsNullOrWhiteSpace(options.ConnectionString)
                ? "localhost:6379"
                : options.ConnectionString;

            var redisConfiguration = ConfigurationOptions.Parse(connectionString);
            redisConfiguration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });

        return services;
    }
}
