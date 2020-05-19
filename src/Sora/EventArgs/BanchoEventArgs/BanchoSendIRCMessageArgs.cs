using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoSendIrcMessageArgs : IEventArgs, INeedPresence
    {
        public MessageStruct Message;
        public Presence Pr { get; set; }
    }
}
