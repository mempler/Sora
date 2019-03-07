using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoFriendAddArgs : IEventArgs, INeedPresence
    {
        public int FriendId;
        public Presence pr { get; set; }
    }
}