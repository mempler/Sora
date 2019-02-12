using EventManager.Attributes;
using EventManager.Enums;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnUserStatsRequestEvent
    {
        private readonly PresenceService _ps;

        public OnUserStatsRequestEvent(PresenceService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoUserStatsRequest)]
        public void OnUserStatsRequest(BanchoUserStatsRequestArgs args)
        {
            foreach (int id in args.userIds)
            {
                if (id == args.pr.User.Id)
                {
                    args.pr.Write(new UserPresence(args.pr));
                    continue;
                }

                Presence opr = _ps.GetPresence(id);
                if (opr == null)
                {
                    args.pr.Write(new HandleUserQuit(new UserQuitStruct
                    {
                        UserId     = id,
                        ErrorState = ErrorStates.Ok
                    }));
                    continue;
                }

                args.pr.Write(new UserPresence(opr));
            }
        }
    }
}