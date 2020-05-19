using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchTransferHostEvent
    {
        [Event(EventType.BanchoMatchTransferHost)]
        public void OnBanchoMatchTransferHost(BanchoMatchTransferHostArgs args)
        {
            if (args.Pr.ActiveMatch == null ||
                args.Pr.ActiveMatch.HostId != args.Pr.User.Id)
                return;
            if (args.SlotId > 16)
                return;
            var slot = args.Pr.ActiveMatch.Slots[args.SlotId];

            args.Pr.ActiveMatch.SetHost(slot.UserId);
            args.Pr.ActiveMatch.Update();
        }
    }
}
