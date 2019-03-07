using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoInviteArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
        public int UserId;
    }
}