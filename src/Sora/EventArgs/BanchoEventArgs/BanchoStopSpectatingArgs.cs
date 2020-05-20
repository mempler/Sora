using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoStopSpectatingArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
