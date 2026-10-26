namespace AngularApp3.Server.Configuration;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = "localhost:6379";
    public string KeyPrefix { get; set; } = "angularapp3:products:catalog:";
    public int CatalogTtlSeconds { get; set; } = 90;
}
