using Modules.Users.Domain.Permissions.ValueObjects;

namespace Modules.Users.Application.Security;

public static class PermissionCatalog
{
    public static readonly PermissionName TweetCreate = PermissionName.Create("tweet.create");
    public static readonly PermissionName TweetLike = PermissionName.Create("tweet.like");
    public static readonly PermissionName TweetDelete = PermissionName.Create("tweet.delete");
    public static readonly PermissionName ProfileEdit = PermissionName.Create("profile.edit");
    
    public static readonly PermissionName UserMute = PermissionName.Create("user.mute");
    
    public static readonly PermissionName UserBan = PermissionName.Create("user.ban");
    public static readonly PermissionName UserDelete = PermissionName.Create("user.delete");
    public static readonly PermissionName PimAccess = PermissionName.Create("pim.access");
    
}