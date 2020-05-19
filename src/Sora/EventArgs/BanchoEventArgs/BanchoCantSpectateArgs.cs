using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoCantSpectateArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
