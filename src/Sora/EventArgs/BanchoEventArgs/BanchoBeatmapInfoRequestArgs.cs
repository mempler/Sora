using System.Collections.Generic;
using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoBeatmapInfoRequestArgs : IEventArgs, INeedPresence
    {
        public List<string> FileNames { get; set; }
        public Presence pr { get; set; }
    }
}