using System;
using EventManager.Attributes;
using EventManager.Enums;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchChangeTeamEvent
    {
        [Event(EventType.BanchoMatchChangeTeam)]
        public void OnBanchoMatchChangeTeam(BanchoMatchChangeTeamArgs args)
        {
            MultiplayerSlot slot = args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id);
            if (slot == null) return;

            switch (slot.Team)
            {
                case MultiSlotTeam.Blue:
                    slot.Team = MultiSlotTeam.Red;
                    break;
                case MultiSlotTeam.Red:
                    slot.Team = MultiSlotTeam.Blue;
                    break;
                case MultiSlotTeam.NoTeam:
                    slot.Team = new Random().Next(1) == 1 ? MultiSlotTeam.Red : MultiSlotTeam.Blue;
                    break;
                default:
                    slot.Team = MultiSlotTeam.NoTeam;
                    break;
            }

            args.pr.JoinedRoom.Update();
        }
    }
}