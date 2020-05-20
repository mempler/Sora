using Sora.Framework.Objects;
using Sora.Framework.Packets.Client;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchScoreUpdateArgs : INeedPresence, IEventArgs
    {
        public ScoreFrame Frame;
        public Presence Pr { get; set; }
    }
}
