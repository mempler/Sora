using System;
using System.Collections.Generic;
using System.Linq;
using Sora.Framework.Packets.Server;

namespace Sora.Framework.Objects
{
    [Flags]
    public enum ChannelStatus
    {
        AdminOnly = 1,
        AutoJoin = 1<<1,
        ReadOnly = 1<<2
    }
    
    public class Channel : PresenceKeeper
    {
        public string Name;
        public string Topic;
        public ChannelStatus Status;

        public IEnumerable<Presence> Presences
        {
            get
            {
                try
                {
                    RWL.AcquireReaderLock(50);
                    return Values.Select(x => x.Value);
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }
        
        public int UserCount
        {
            get
            {
                try
                {
                    RWL.AcquireReaderLock(50);
                    return Values.Count;
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }

        public override void Join(Presence sender)
        {
            if ((Status & ChannelStatus.ReadOnly) != 0 &&
                sender.Permission != Permission.ADMIN_CHANNEL_READONLY)
            {
                // TODO: Bot Callback
                return;
            }

            if ((Status & ChannelStatus.AdminOnly) != 0 &&
                sender.Permission != Permission.ADMIN_CHANNEL)
            {
                sender.Push(new ChannelRevoked(this));
                return;
            }
            base.Join(sender);
        }
    }
}