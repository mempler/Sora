using Sora.Framework.Enums;
using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoExitArgs : IEventArgs, INeedPresence
    {
        public ErrorStates Err;
        public Presence Pr { get; set; }
    }
}
