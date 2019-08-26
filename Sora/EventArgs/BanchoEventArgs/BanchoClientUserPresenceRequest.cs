using System.Collections.Generic;
using Sora.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    
    public class BanchoClientUserPresenceRequestArgs : IEventArgs, INeedPresence
    {
        public List<int> userIds;
        public Presence pr { get; set; }
    }
}