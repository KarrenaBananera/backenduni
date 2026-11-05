namespace AngularApp3.Server.Configuration;

public class SupabaseStorageOptions
{
    public const string SectionName = "Supabase";

    public string Url { get; set; } = string.Empty;
    public string ServiceRoleKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = "products";
}
