using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoStartSpectatingArgs : IEventArgs, INeedPresence
    {
        public int SpectatorHostId;
        public Presence Pr { get; set; }
    }
}
