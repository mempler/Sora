using System;

namespace Sora.Framework.Enums
{
    [Flags]
    public enum LoginPermissions
    {
        User = 1 << 0,
        BAT = 1 << 1,
        Supporter = 1 << 2,
        Moderator = 1 << 3,
        Developer = 1 << 4,
        Administrator = 1 << 5,
        TorneyStaff = 1 << 6
    }
}
