using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;
using Sora.Utilities;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnPrivateMessageEvent
    {
        private readonly PresenceService _ps;
        private readonly ChatFilter _filter;

        public OnPrivateMessageEvent(PresenceService ps, ChatFilter filter)
        {
            _ps = ps;
            _filter = filter;
        }

        [Event(EventType.BanchoSendIrcMessagePrivate)]
        public void OnPrivateMessage(BanchoSendIrcMessageArgs args)
        {
            if (!_ps.TryGet(args.Message.ChannelTarget, out var target)) {
                args.Pr.Push(new Announce("This User is Offline!"));
                return;
            }
  
            Logger.Info(
                $"{LCol.RED}{args.Pr.User.UserName}",
                $"{LCol.PURPLE}( {args.Pr.User.Id} )",
                $"{LCol.YELLOW}(Private Message)",
                $"{LCol.WHITE}=>",
                $"{LCol.RED}{args.Pr.User.UserName}",
                $"{LCol.PURPLE}( {args.Pr.User.Id} )"
            );

            var newMsg = new MessageStruct
            {
                Username = args.Pr.User.UserName,
                Message = _filter.Filter(args.Message.Message),
                ChannelTarget = args.Pr.User.UserName,
                SenderId = args.Pr.User.Id
            };
            
            target.Push(new SendIrcMessage(newMsg));
        }
    }
}
