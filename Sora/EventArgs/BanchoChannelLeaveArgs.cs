using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoChannelLeaveArgs : IEventArgs, INeedPresence
    {
        public string ChannelName;
        public Presence pr { get; set; }
    }
}