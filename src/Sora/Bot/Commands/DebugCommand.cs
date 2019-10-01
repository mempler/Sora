using System.Collections.Generic;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework;
using Sora.Framework.Objects;

namespace Sora.Bot.Commands
{
    [EventClass]
    public class DebugCommand : ISoraCommand
    {
        public string Command => "debug";
        public string Description => "Debug packets";

        public List<Argument> Args => new List<Argument>();

        public int ExpectedArgs => 0;

        public Permission RequiredPermission => Permission.From(Permission.GROUP_DEVELOPER);

        public bool Execute(Presence executor, string[] args)
        {
            if (executor["IS_PACKET_DEBUGGING"] == null)
                executor["IS_PACKET_DEBUGGING"] = false;

            executor["IS_PACKET_DEBUGGING"] = !(bool) executor["IS_PACKET_DEBUGGING"];

            executor.Alert("Debugger has been " + ((bool) executor["IS_PACKET_DEBUGGING"] ? "Enabled" : "Disabled"));

            return false;
        }

        [Event(EventType.BanchoPacket)]
        public void OnBanchoPacketEvent(BanchoPacketArgs args)
        {
            if (args.pr["IS_PACKET_DEBUGGING"] == null)
                return;
            
            if (!(bool) args.pr["IS_PACKET_DEBUGGING"])
                args.pr.Alert($"PacketId: {args.PacketId}");
        }
    }
}
