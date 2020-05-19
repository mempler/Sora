using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoChannelJoinArgs : IEventArgs, INeedPresence
    {
        public string ChannelName;
        public Presence Pr { get; set; }
    }
}
