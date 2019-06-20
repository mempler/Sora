using Sora.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class EventArgs<T> : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
        public T Data { get; set; }
    }
}