namespace AngularApp3.Server.Identity;

public static class AppRoles
{
    public const string User = "user";
    public const string Admin = "admin";

    public static readonly IReadOnlyList<string> All = [User, Admin];
}
