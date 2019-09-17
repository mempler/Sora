using System.Collections.Generic;
using System.Linq;
using Sora.Framework;
using Sora.Framework.Objects;

namespace Sora.Services
{
    public class PresenceService : PresenceKeeper
    {

        public int ConnectedPresences => Values.Count;
        
        public IEnumerable<int> GetUserIds(Presence pr = null)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                return Values.Where(x => x.Value != pr).Select(x => x.Value.User.Id);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }
    }
}