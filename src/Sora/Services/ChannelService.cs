using System.Collections.Generic;
using System.Linq;
using Sora.Framework;
using Sora.Framework.Objects;

namespace Sora.Services
{
    public class ChannelService : AsyncKeeper<string, Channel>
    {
        public IEnumerable<Channel> ChannelsAutoJoin 
        {
            get
            {
                try
                {
                    RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                    return Values.Where(x => (x.Value.Status & ChannelStatus.AutoJoin) != 0).Select(x => x.Value);
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }

        public IEnumerable<Channel> Channels
        {
            get
            {
                try
                {
                    RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                    return Values.Select(x => x.Value);
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }
    }
}