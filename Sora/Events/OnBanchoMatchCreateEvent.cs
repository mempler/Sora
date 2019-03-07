using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchCreateEvent
    {
        private readonly MultiplayerService _ms;

        public OnBanchoMatchCreateEvent(MultiplayerService ms)
        {
            _ms = ms;
        }

        
        [Event(EventType.BanchoMatchCreate)]
        public void OnBanchoMatchCreate(BanchoMatchCreateArgs args)
        {
            args.room.Password = args.room.Password.Replace(" ", "_");
            _ms.Add(args.room);

            if (args.room.Join(args.pr, args.room.Password))
                args.pr.Write(new MatchJoinSuccess(args.room));
            else
                args.pr.Write(new MatchJoinFail());

            args.room.Update();
        }
    }
}