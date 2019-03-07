using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchSkipRequestEvent
    {
        [Event(EventType.BanchoMatchSkipRequest)]
        public void OnBanchoMatchSkipRequest(BanchoMatchSkipRequestArgs args)
        {
            args.pr.JoinedRoom?.Skip(args.pr);
        }
    }
}