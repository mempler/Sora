using EventManager.Interfaces;
using Shared.Helpers;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoLoginRequestArgs : IEventArgs, INeedPresence
    {
        public MStreamWriter Writer;
        public MStreamReader Reader;
        public string IPAddress;
        
        public Presence pr { get; set; }
    }
}