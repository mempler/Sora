using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchChangeTeamArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
