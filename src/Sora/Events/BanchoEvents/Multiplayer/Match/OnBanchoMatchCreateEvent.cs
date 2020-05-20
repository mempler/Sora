using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchCreateEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchCreateEvent(EventManager ev)
        {
            _ev = ev;
        }

        [Event(EventType.BanchoMatchCreate)]
        public async void OnBanchoMatchCreate(BanchoMatchCreateArgs args)
        {
            args.Room.Password = args.Room.Password.Replace(" ", "_");
            
            Lobby.Self.Push(args.Room);
            
            if (args.Room.Join(args.Pr, args.Room.Password))
                args.Pr.Push(new MatchJoinSuccess(args.Room));
            else
                args.Pr.Push(new MatchJoinFail());

            args.Room.Update();

            Logger.Info(
                "%#F94848%" + args.Pr.User.UserName,
                "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                "%#FFFFFF%has created a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" + args.Room.Name,
                "%#B342F4%(", args.Room.MatchId, "%#B342F4%)"
            );
            
            await _ev.RunEvent(
                EventType.BanchoChannelJoin, new BanchoChannelJoinArgs {Pr = args.Pr, ChannelName = "#multiplayer"}
            );
        }
    }
}
