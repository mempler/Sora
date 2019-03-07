using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchLoadCompleteEvent
    {
        [Event(EventType.BanchoMatchLoadComplete)]
        public void OnBanchoMatchLoadComplete(BanchoMatchLoadCompleteArgs args)
        {
            if (args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id) != null)
                args.pr.JoinedRoom.LoadComplete();
        }
    }
}