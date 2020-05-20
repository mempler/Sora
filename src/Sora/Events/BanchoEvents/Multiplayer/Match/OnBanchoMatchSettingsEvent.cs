using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchSettingsEvent
    {
        [Event(EventType.BanchoMatchChangeSettings)]
        public void OnBroadcastFrames(BanchoMatchChangeSettingsArgs args)
        {
            if (args.Pr.ActiveMatch == null)
                return;
            if (args.Pr.ActiveMatch.HostId != args.Pr.User.Id)
                return;

            if (args.Pr.ActiveMatch.Name != args.Room.Name)
                Logger.Info(
                    "%#F94848%" + args.Pr.User.UserName,
                    "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                    "%#FFFFFF% renamed a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" +
                    args.Pr.ActiveMatch.Name,
                    "%#B342F4%(", args.Room.MatchId, "%#B342F4%)",
                    "%#FFFFFF%and is now called %#F94848%" +
                    args.Room.Name,
                    "%#B342F4%(", args.Room.MatchId, "%#B342F4%)"
                );

            Logger.Info(
                "%#F94848%" + args.Pr.User.UserName,
                "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                "%#FFFFFF%changed the Settings of a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" +
                args.Room.Name,
                "%#B342F4%(", args.Room.MatchId, "%#B342F4%)"
            );

            args.Pr.ActiveMatch.ChangeSettings(args.Room);
            args.Pr.ActiveMatch.Update();
        }
    }
}
