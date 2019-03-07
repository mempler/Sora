using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Client;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchSettingsEvent
    {
        [Event(EventType.BanchoMatchChangeSettings)]
        public void OnBroadcastFrames(BanchoMatchChangeSettingsArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            if (args.pr.JoinedRoom.HostId != args.pr.User.Id) return;

            args.pr.JoinedRoom.ChangeSettings(args.room);
        }
    }
}