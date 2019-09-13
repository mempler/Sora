using System;
using Sora.Framework.Packets.Server;

namespace Sora.Framework.Objects
{
    [Flags]
    public enum ChannelStatus
    {
        AdminOnly = 1<<0,
        AutoJoin = 1<<1,
        ReadOnly = 1<<2
    }
    
    public class Channel : PresenceKeeper
    {
        public string Name;
        public string Topic;
        public ChannelStatus Status;

        public int UserCount
        {
            get
            {
                try
                {
                    RWL.AcquireReaderLock(50);
                    return Presences.Count;
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }

        public void SendMessage(Presence sender, string msg)
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
                // TODO: Revoke Channel
                return;
            }
            
            Push(new SendIrcMessage(new MessageStruct
            {
                Message = msg,
                Username = sender.User.UserName,
                ChannelTarget = Name,
                SenderId = sender.User.Id
            }));
        }
    }
}