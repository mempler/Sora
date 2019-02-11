using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoFriendRemoveArgs : IEventArgs, INeedPresence
    {
        public int FriendId;
        public Presence pr { get; set; }
    }
}