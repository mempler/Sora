using System.Linq;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;
using Sora.Framework.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnUserStatsRequestEvent
    {
        private readonly PresenceService _ps;

        public OnUserStatsRequestEvent(PresenceService ps) => _ps = ps;

        [Event(EventType.BanchoUserStatsRequest)]
        public void OnUserStatsRequest(BanchoUserStatsRequestArgs args)
        {
            foreach (var id in args.UserIds.Where(id => id != args.Pr.User.Id))
            {
                if (!_ps.TryGet(id, out var opr))
                {
                    args.Pr.Push(new HandleUserQuit(new UserQuitStruct {UserId = id, ErrorState = ErrorStates.Ok}));
                    continue;
                }

                args.Pr.Push(new HandleUpdate(opr));
            }
        }
    }
}
