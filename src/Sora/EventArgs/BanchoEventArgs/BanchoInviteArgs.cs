using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoInviteArgs : IEventArgs, INeedPresence
    {
        public int UserId;
        public Presence Pr { get; set; }
    }
}
