using EventManager.Interfaces;
using Sora.Enums;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoExitArgs : IEventArgs, INeedPresence
    {
        public ErrorStates err;
        public Presence pr { get; set; }
    }
}