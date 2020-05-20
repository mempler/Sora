using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoLobbyPartArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
