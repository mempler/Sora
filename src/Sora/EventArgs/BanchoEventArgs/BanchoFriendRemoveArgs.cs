using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoFriendRemoveArgs : IEventArgs, INeedPresence
    {
        public int FriendId;
        public Presence Pr { get; set; }
    }
}
