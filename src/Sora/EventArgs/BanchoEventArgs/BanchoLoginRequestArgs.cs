using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoLoginRequestArgs : IEventArgs, INeedPresence
    {
        public string IpAddress;
        public MStreamReader Reader;
        public MStreamWriter Writer;

        public Presence Pr { get; set; }
    }
}
