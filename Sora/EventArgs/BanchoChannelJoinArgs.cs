using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoChannelJoinArgs : IEventArgs, INeedPresence
    {
        public string ChannelName;
        public Presence pr { get; set; }
    }
}