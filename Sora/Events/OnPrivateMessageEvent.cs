using EventManager.Attributes;
using EventManager.Enums;
using Shared.Models;
using Shared.Services;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnPrivateMessageEvent
    {
        private readonly Database _db;
        private readonly PresenceService _ps;

        public OnPrivateMessageEvent(Database db, PresenceService ps, ChannelService cs)
        {
            _db = db;
            _ps = ps;
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessage(BanchoSendIRCMessageArgs args)
        {
            Presence opr = _ps.GetPresence(Users.GetUserId(_db, args.Message.ChannelTarget));
            if (opr == null) return;
            Channel chan = opr.PrivateChannel;

            if (chan == null)
            {
                args.pr.Write(new ChannelRevoked(args.Message.ChannelTarget));
                return;
            }

            chan.WriteMessage(args.pr, args.Message.Message);
        }
    }
}