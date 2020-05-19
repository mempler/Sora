using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnBanchoExitEvent
    {
        private readonly PresenceService _ps;
        
        public OnBanchoExitEvent(PresenceService ps)
        {
            _ps = ps;
        }

        [Event(EventType.BanchoExit)]
        public void OnBanchoExit(BanchoExitArgs args)
        {
            Logger.Info(
                "%#F94848%" + args.Pr.User.UserName,
                "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                "%#FFFFFF%has Disconnected!"
            );
            
            args.Pr.Spectator?.Leave(args.Pr);
            args.Pr.ActiveMatch?.Leave(args.Pr);

            _ps.Push(new HandleUserQuit(new UserQuitStruct {UserId = args.Pr.User.Id, ErrorState = args.Err}));

            _ps.Pop(args.Pr.Token);
        }
    }
}
