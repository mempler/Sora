using System.Collections.Generic;
using System.Net;
using EventManager.Interfaces;

namespace Jibril.EventArgs
{
    public class SharedEventArgs : IEventArgs
    {
        public HttpListenerRequest req;
        public HttpListenerResponse res;
        public Dictionary<string, string> args;
    }
}