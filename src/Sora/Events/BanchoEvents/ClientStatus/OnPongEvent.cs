using System;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnPongEvent
    {
        [Event(EventType.BanchoPong)]
        public void OnPong(BanchoPongArgs args)
        {
            args.Pr["LAST_PONG"] = DateTime.Now;
                
            if ((args.Pr.Spectator == null ||
                args.Pr.Spectator?.Host != args.Pr) ||
                args.Pr.Spectator?.SpectatorCount <= 0)
                return;
            
            args.Pr.Spectator.Push(new HandleUpdate(args.Pr.Spectator.Host));
        }
    }
}
