using System.Collections.ObjectModel;

namespace Common.Authorization;

public record AppPermission(string Feature,string Action,string Group,string Description,bool IsBasic = false)
{
    public string Name => NameFor(Feature, Action);
    public static string NameFor(string feature,string action)
    => $"Permission.{feature}.{action}";
}

public class AppPermissions
{
    private static readonly AppPermission[] _all = new AppPermission[]
    {
        new AppPermission(AppFeature.Users,AppActions.Read,AppRoleGroup.SystemAccess,"Read Users"),
        new AppPermission(AppFeature.Users,AppActions.Create,AppRoleGroup.SystemAccess,"Create Users"),
        new AppPermission(AppFeature.Users,AppActions.Update,AppRoleGroup.SystemAccess,"Update Users"),
        new AppPermission(AppFeature.Users,AppActions.Delete,AppRoleGroup.SystemAccess,"Delete Users"),
        
        new AppPermission(AppFeature.UserRoles,AppActions.Create,AppRoleGroup.SystemAccess,"Create UserRoles"),
        new AppPermission(AppFeature.UserRoles,AppActions.Update,AppRoleGroup.SystemAccess,"Update UserRoles"),
        new AppPermission(AppFeature.UserRoles,AppActions.Read,AppRoleGroup.SystemAccess,"Read UserRoles"),
        new AppPermission(AppFeature.UserRoles,AppActions.Delete,AppRoleGroup.SystemAccess,"Delete UserRoles"),
        
        new AppPermission(AppFeature.Roles,AppActions.Read,AppRoleGroup.SystemAccess,"Read Roles"),
        new AppPermission(AppFeature.Roles,AppActions.Update,AppRoleGroup.SystemAccess,"Update Roles"),
        
        new AppPermission(AppFeature.RoleClaims,AppActions.Read,AppRoleGroup.SystemAccess,"Read Role claims/permissions"),
        new AppPermission(AppFeature.RoleClaims,AppActions.Update,AppRoleGroup.SystemAccess,"Update Role claims/permissins"),
        
        new AppPermission(AppFeature.Employees,AppActions.Read,AppRoleGroup.SystemAccess,"Read Employees",IsBasic: true ),
        new AppPermission(AppFeature.Employees,AppActions.Create,AppRoleGroup.SystemAccess,"Create Employees"),
        new AppPermission(AppFeature.Employees,AppActions.Update,AppRoleGroup.SystemAccess,"Update Employees"),
        new AppPermission(AppFeature.Employees,AppActions.Delete,AppRoleGroup.SystemAccess,"Delete Employees"),
    };

    public static IReadOnlyList<AppPermission> AdminPermissions
        => new ReadOnlyCollection<AppPermission>(_all.ToArray());

    public static IReadOnlyList<AppPermission> BasicPermissions
        => new ReadOnlyCollection<AppPermission>(_all.Where(per => per.IsBasic).ToArray());
}