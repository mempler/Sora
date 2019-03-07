using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchCompleteArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}