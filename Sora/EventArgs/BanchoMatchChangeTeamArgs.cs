using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchChangeTeamArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}