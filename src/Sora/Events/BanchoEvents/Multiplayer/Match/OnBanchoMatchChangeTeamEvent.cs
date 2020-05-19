using System;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchChangeTeamEvent
    {
        [Event(EventType.BanchoMatchChangeTeam)]
        public void OnBanchoMatchChangeTeam(BanchoMatchChangeTeamArgs args)
        {
            var slot = args.Pr.ActiveMatch?.GetSlotByUserId(args.Pr.User.Id);
            if (slot == null)
                return;

            slot.Team = slot.Team switch
            {
                MultiSlotTeam.Blue => MultiSlotTeam.Red,
                MultiSlotTeam.Red => MultiSlotTeam.Blue,
                MultiSlotTeam.NoTeam => (new Random().Next(1) == 1 ? MultiSlotTeam.Red : MultiSlotTeam.Blue),
                _ => MultiSlotTeam.NoTeam
            };

            args.Pr.ActiveMatch.Update();
        }
    }
}
