namespace Common.Authorization;

public static class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles => [Admin, Basic];

    public static bool IsDefaultRole(string roleName) => DefaultRoles.Any(r => r == roleName);
}