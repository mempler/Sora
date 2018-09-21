using System;
using System.Collections.Generic;
using System.IO;
using Sora.Helpers;
using Sora.Packets;

namespace Sora.Objects
{
    public static class Presences
    {
        private static readonly Dictionary<string, Presence> presences = new Dictionary<string, Presence>();

        public static Presence GetPresence(string token) => presences.TryGetValue(token, out var pr) ? pr : null;

        public static void BeginPresence(Presence presence)
        {
            presence.BeginSeason = DateTime.Now;
            presences[presence.Token] = presence;
        }
        public static void EndPresence(Presence presence, bool forceful)
        {
            if (forceful && presences.ContainsKey(presence.Token))
            {
                presences[presence.Token] = null;
                return;
            }

            presence.LastRequest = true;
            presence.Stream.Write(new Announce("Your session has ended!"));
        }
    }
    public class Presence
    {
        public string Token;
        public MStreamWriter Stream;
        public DateTime BeginSeason;
        public bool LastRequest = false;

        private readonly MemoryStream _str;

        public Presence()
        {
            Token = Guid.NewGuid().ToString();
            _str = new MemoryStream();
            Stream = new MStreamWriter(_str);
        }
    }
}
