using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchStartEvent
    {
        [Event(EventType.BanchoMatchStart)]
        public void OnBanchoMatchStart(BanchoMatchStartArgs args)
        {
            if (args.Pr.ActiveMatch == null ||
                args.Pr.ActiveMatch.HostId != args.Pr.User.Id)
                return;

            Logger.Info(
                "%#FFFFFF% a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" +
                args.Pr.ActiveMatch.Name,
                "%#B342F4%(", args.Pr.ActiveMatch.MatchId, "%#B342F4%) %#FFFFFF%has started their Match!"
            );

            args.Pr.ActiveMatch.Start();
            args.Pr.ActiveMatch.Update();
        }
    }
}
