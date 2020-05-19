using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchPartEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchPartEvent(EventManager ev) => _ev = ev;

        [Event(EventType.BanchoMatchPart)]
        public async void OnBanchoMatchPart(BanchoMatchPartArgs args)
        {
            if (args.Pr.ActiveMatch == null)
                return;

            var room = args.Pr.ActiveMatch;
            room.Leave(args.Pr);
            if (room.HostId == args.Pr.User.Id)
                room.SetRandomHost();

            await _ev.RunEvent(
                EventType.BanchoChannelLeave, new BanchoChannelLeaveArgs {Pr = args.Pr, ChannelName = "#multiplayer"}
            );

            if (room.HostId != -1)
            {
                room.Update();
                return;
            }

            Logger.Info(
                "Detected Empty %#f1fc5a%Multiplayer Room %#FFFFFF%called",
                "%#F94848%" + room.Name,
                "%#B342F4%(", room.MatchId, "%#B342F4%)%#FFFFFF% Cleaning up..."
            );

            room.Disband();
        }
    }
}
