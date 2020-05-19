using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoPongArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
