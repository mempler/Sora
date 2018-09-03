using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kaoiji.objects
{
    public class Presences
    {
        public static Dictionary<string, Presence> presences = new Dictionary<string, Presence>();

        public static Presence GetPresence(string Token)
        {
            if (Token == null && !presences.ContainsKey(Token))
                return null;
            return presences[Token];
        }
    }

    public class Presence
    {
        private MemoryStream OutputStream;

        public string Token;

        public Presence()
        {
            Token = Guid.NewGuid().ToString();
            OutputStream = new MemoryStream();
        }
    }
}
