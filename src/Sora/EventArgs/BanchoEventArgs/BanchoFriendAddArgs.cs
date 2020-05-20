using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoFriendAddArgs : IEventArgs, INeedPresence
    {
        public int FriendId;
        public Presence Pr { get; set; }
    }
}
