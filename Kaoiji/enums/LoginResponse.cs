using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaoiji.enums
{
    public enum LoginResponses
    {
        FAILED = -1,
        OUTDATED = -2,
        BANNED = -3,
        MULTIACC = -4,
        EXCEPTION = -5,
        SUPPORTERONLY = -6,
        TWOFACTORAUTH = -8,
    }
}
