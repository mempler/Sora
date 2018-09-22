using System;
using System.Collections.Generic;
using System.IO;
using Shared.Database.Models;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Packets.Server;

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
        public bool LastRequest;

        public Users User;
        public bool BlockNonFriendDm;
        public int Timezone;

        public Presence()
        {
            Token = Guid.NewGuid().ToString();
            var str = new MemoryStream();
            Stream = new MStreamWriter(str);
        }

        public void Write(IPacketSerializer p) => Stream.Write(p);
    }
}
