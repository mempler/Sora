#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Packets.Client;

namespace Sora.Objects
{
    public static class Presences
    {
        private static readonly Dictionary<string, Presence> presences = new Dictionary<string, Presence>();
        public static int UserCount;

        public static Presence GetPresence(string token) => presences.TryGetValue(token, out var pr) ? pr : null;
        public static Presence GetPresence(int userid)
        {
            foreach (var presence in presences)
                if (presence.Value.User.Id == userid)
                    return presence.Value;
            return null;
        }

        public static IEnumerable<int> GetUserIds()
        {
            foreach (var presence in presences)
                yield return presence.Value.User.Id;
        }
        public static IEnumerable<int> GetUserIds(Presence pr)
        {
            foreach (var presence in presences)
            {
                if (presence.Value.User.Id == pr.User.Id) continue;
                yield return presence.Value.User.Id;
            }
        }
        
        public static void BeginPresence(Presence presence)
        {
            presence.BeginSeason = DateTime.Now;
            presences[presence.Token] = presence;
            UserCount++;
        }
        public static void EndPresence(Presence presence, bool forceful)
        {
            if (forceful && presences.ContainsKey(presence.Token))
            {
                presences[presence.Token] = null;
                return;
            }

            presence.LastRequest = true;
            UserCount--;
        }
    }
    public class Presence
    {
        public string Token;
        public MStreamWriter Stream;
        public DateTime BeginSeason;
        public bool LastRequest;

        public Users User;
        public UserStats Stats;
        public LeaderboardStd LeaderboardStd;
        public LeaderboardRx LeaderboardRx;
        public LeaderboardTouch LeaderboardTouch;
        public UserStatus Status = new UserStatus { BeatmapChecksum = "", StatusText = "" }; // Predefined strings to prevent Issues.

        public uint Rank;

        public bool BlockNonFriendDm;
        public int ClientPermissions;

        public CountryIds CountryId;
        public double Lon;
        public double Lat;
        public byte Timezone;

        public bool Touch;
        public bool Relax;
        public bool Disconnected;

        public Presence()
        {
            Token = Guid.NewGuid().ToString();
            var str = new MemoryStream();
            Stream = new MStreamWriter(str);
        }

        protected bool Equals(Presence pr) => Token == pr.Token;


        public void Write(IPacketSerializer p) => Stream.Write(p);
    }
}
