using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchChangeModsEvent
    {
        [Event(EventType.BanchoMatchChangeMods)]
        public void OnBanchoMatchChangeMods(BanchoMatchChangeModsArgs args)
        {
            var slot = args.Pr.ActiveMatch?.GetSlotByUserId(args.Pr.User.Id);
            if (slot == null)
                return;
            
            args.Pr.ActiveMatch.SetMods(args.Mods, slot);
            args.Pr.ActiveMatch.Update();
        }
    }
}
