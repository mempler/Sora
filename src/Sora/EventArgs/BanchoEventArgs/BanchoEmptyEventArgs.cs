using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoEmptyEventArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}
