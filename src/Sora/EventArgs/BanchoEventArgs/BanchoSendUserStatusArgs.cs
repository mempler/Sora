using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoSendUserStatusArgs : IEventArgs, INeedPresence
    {
        public UserStatus Status;
        public Presence Pr { get; set; }
    }
}
