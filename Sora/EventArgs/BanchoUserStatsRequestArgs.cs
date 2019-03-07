using System.Collections.Generic;
using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoUserStatsRequestArgs : IEventArgs, INeedPresence
    {
        public List<int> userIds;
        public Presence pr { get; set; }
    }
}