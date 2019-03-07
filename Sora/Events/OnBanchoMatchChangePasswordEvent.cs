using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchChangePasswordEvent
    {
        [Event(EventType.BanchoMatchChangePassword)]
        public void OnBanchoMatchChangePassword(BanchoMatchChangePasswordArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            if (args.pr.JoinedRoom.HostId != args.pr.User.Id) return;

            args.pr.JoinedRoom.SetPassword(args.room.Password);
        }
    }
}