using Sora.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoEmptyEventArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}