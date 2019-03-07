using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchPartEvent
    {    
        [Event(EventType.BanchoMatchPart)]
        public void OnBanchoMatchPart(BanchoMatchPartArgs args)
        {
            if (args.pr.JoinedRoom == null) return;

            MultiplayerRoom room = args.pr.JoinedRoom;
            room.Leave(args.pr);
            if (room.HostId == args.pr.User.Id)
                room.SetRandomHost();

            if (room.HostId != -1)
            {
                room.Update();
                return;
            }

            room.Dispand();
        }
    }
}