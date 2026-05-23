using Modules.Users.Domain.Permissions.ValueObjects;

namespace Modules.Users.Application.Security;

public static class PermissionCatalog
{
    public static readonly PermissionName TweetCreate = PermissionName.From("tweet.create");
    public static readonly PermissionName TweetLike = PermissionName.From("tweet.like");
    public static readonly PermissionName TweetDelete = PermissionName.From("tweet.delete");
    public static readonly PermissionName ProfileEdit = PermissionName.From("profile.edit");

    public static readonly PermissionName UserMute = PermissionName.From("user.mute");

    public static readonly PermissionName UserBan = PermissionName.From("user.ban");
    public static readonly PermissionName UserDelete = PermissionName.From("user.delete");
    public static readonly PermissionName PimAccess = PermissionName.From("pim.access");

}
