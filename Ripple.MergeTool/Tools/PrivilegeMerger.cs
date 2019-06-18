using Ripple.MergeTool.Enums;
using Sora.Enums;

namespace Ripple.MergeTool.Tools
{
    public static class PrivilegeMerger
    {
        public static Privileges Merge(RipplePrivileges priv)
        {
            Privileges retVal = 0;

            if ((priv & RipplePrivileges.USER_NORMAL) != 0)
                retVal |= Privileges.Default;
            if ((priv & RipplePrivileges.ADMIN_SILENCE_USERS) != 0)
                retVal |= Privileges.CModSilence;
            if ((priv & RipplePrivileges.ADMIN_MANAGE_USERS) != 0)
                retVal |= Privileges.ARestrict;
            if ((priv & RipplePrivileges.USER_DONOR) != 0)
                retVal |= Privileges.Donator;
            if ((priv & RipplePrivileges.ADMIN_CHAT_MOD) != 0)
                retVal |= Privileges.CMod;

            return retVal;
        }
    }
}