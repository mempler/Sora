using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;
using Sora.Utilities;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnPublicMessageEvent
    {
        private readonly ChannelService _cs;
        private readonly ChatFilter _filter;

        public OnPublicMessageEvent(ChannelService cs, ChatFilter filter)
        {
            _cs = cs;
            _filter = filter;
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessage(BanchoSendIrcMessageArgs args)
        {
            Channel channel;
            
            switch (args.Message.ChannelTarget)
            {
                case "#spectator":
                    channel = args.Pr.Spectator?.Channel;
                    break;
                case "#multiplayer":
                    channel = args.Pr.ActiveMatch?.Channel;
                    break;
                default:
                    _cs.TryGet(args.Message.ChannelTarget, out channel);
                    break;
            }

            if (channel == null)
            {
                args.Pr.Push(new ChannelRevoked(args.Message.ChannelTarget));
                return;
            }

            Logger.Info(
                $"{LCol.RED}{args.Pr.User.UserName}",
                $"{LCol.PURPLE}( {args.Pr.User.Id} )",
                $"{LCol.YELLOW}{args.Message.Message}",
                $"{LCol.WHITE}=>",
                $"{LCol.RED}{channel.Name}"
            );

            args.Message.Username = args.Pr.User.UserName;
            args.Message.Message = _filter.Filter(args.Message.Message);

            channel.Push(new SendIrcMessage(args.Message), args.Pr);
        }
    }
}
