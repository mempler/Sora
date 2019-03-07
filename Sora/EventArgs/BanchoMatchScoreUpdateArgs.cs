using EventManager.Interfaces;
using Sora.Objects;
using Sora.Packets.Client;

namespace Sora.EventArgs
{
    public class BanchoMatchScoreUpdateArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
        public ScoreFrame Frame;
    }
}