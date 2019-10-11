using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework;
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;

namespace Sora.Bot.Commands
{
    [EventClass]
    public class DebugCommand : ISoraCommand
    {
        private readonly EventManager _ev;
        public string Command => "debug";
        public string Description => "Debug packets";

        public List<Argument> Args => new List<Argument>();

        public int ExpectedArgs => 0;

        public Permission RequiredPermission => Permission.From(Permission.GROUP_DEVELOPER);

        public DebugCommand(EventManager ev)
        {
            _ev = ev;
        }

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

            if ((bool) args.pr["IS_PACKET_DEBUGGING"])
                _ev?.RunEvent(EventType.BanchoSendIrcMessagePrivate, new BanchoSendIRCMessageArgs
                {
                    pr = args.pr,
                    Message = new MessageStruct
                    {
                        Message = $"\n\n\n\n\n\n\n\n\n\n" +
                                  $"\nPacketId: {args.PacketId}" +
                                  $"\nPacket Length: {args.Data.BaseStream.Length}" +
                                  $"\nPacketData: {Hex.ToHex(((MemoryStream) args.Data.BaseStream).ToArray())}",
                        Username = args.pr.User.UserName,
                        ChannelTarget = args.pr.User.UserName,
                        SenderId = args.pr.User.Id
                    }
                });
        }
    }
}
