using EventManager.Interfaces;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.EventArgs
{
    public class BanchoSendIRCMessageArgs : IEventArgs, INeedPresence
    {
        public MessageStruct Message;
        public Presence pr { get; set; }
    }
}