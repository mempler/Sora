using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchChangePasswordEvent
    {
        [Event(EventType.BanchoMatchChangePassword)]
        public void OnBanchoMatchChangePassword(BanchoMatchChangePasswordArgs args)
        {
            if (args.Pr.ActiveMatch?.HostId != args.Pr.User.Id)
                return;

            args.Pr.ActiveMatch.SetPassword(args.Room.Password);
            args.Pr.ActiveMatch.Update();
        }
    }
}
