using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnMatchChangeModsEvent
    {
        [Event(EventType.BanchoMatchChangeMods)]
        public void OnBanchoMatchChangeMods(BanchoMatchChangeModsArgs args)
        {
            MultiplayerSlot slot = args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id);
            if (slot == null) return;
            args.pr.JoinedRoom.SetMods(args.mods, slot);
        }
    }
}