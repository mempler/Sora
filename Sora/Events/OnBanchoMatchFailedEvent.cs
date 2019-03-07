using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchFailedEvent
    {
        [Event(EventType.BanchoMatchFailed)]
        public void OnBanchoMatchFailed(BanchoMatchFailedArgs args)
        {
            args.pr.JoinedRoom?.Failed(args.pr);
        }
    }
}