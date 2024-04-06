using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoChannelLeaveArgs : IEventArgs, INeedPresence
    {
        public string ChannelName;
        public Presence Pr { get; set; }
    }
}
