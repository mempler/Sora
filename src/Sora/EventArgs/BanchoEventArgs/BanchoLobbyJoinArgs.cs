using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoLobbyJoinArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
