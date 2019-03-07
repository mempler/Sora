using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchCompleteEvent
    {
        [Event(EventType.BanchoMatchComplete)]
        public void OnBanchoMatchComplete(BanchoMatchCompleteArgs args)
        {
            args.pr.JoinedRoom?.Complete(args.pr);
        }
    }
}