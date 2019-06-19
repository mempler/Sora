using Ripple.MergeTool.Enums;
using Sora;

namespace Ripple.MergeTool.Tools
{
    public static class PrivilegeMerger
    {
        public static Permission Merge(RipplePrivileges priv)
        {
            Permission retVal = new Permission();

            if ((priv & RipplePrivileges.USER_NORMAL) != 0)
                retVal += Permission.DEFAULT;
            if ((priv & RipplePrivileges.ADMIN_SILENCE_USERS) != 0)
                retVal += Permission.CHAT_MOD_SILENCE;
            if ((priv & RipplePrivileges.ADMIN_MANAGE_USERS) != 0)
                retVal += Permission.ADMIN_RESTRICT;
            if ((priv & RipplePrivileges.USER_DONOR) != 0)
                retVal += Permission.GROUP_DONATOR;
            if ((priv & RipplePrivileges.ADMIN_CHAT_MOD) != 0)
                retVal += Permission.GROUP_CHAT_MOD;

            return retVal;
        }
    }
}