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

namespace Sora.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Enums;
    using JetBrains.Annotations;
    using Packets.Client;
    using Shared.Database.Models;
    using Shared.Enums;
    using Shared.Handlers;
    using Shared.Helpers;
    using Shared.Interfaces;

    public static class LPresences
    {
        private static readonly Dictionary<string, Presence> Presences = new Dictionary<string, Presence>();

        public static Presence GetPresence(string token) => Presences.TryGetValue(token, out Presence pr) ? pr : null;

        public static Presence GetPresence(int userid)
        {
            foreach (KeyValuePair<string, Presence> presence in Presences)
                if (presence.Value.User.Id == userid)
                    return presence.Value;

            return null;
        }

        [UsedImplicitly]
        public static IEnumerable<int> GetUserIds()
        {
            foreach (KeyValuePair<string, Presence> presence in Presences)
                yield return presence.Value.User.Id;
        }

        public static IEnumerable<int> GetUserIds(Presence pr)
        {
            foreach (KeyValuePair<string, Presence> presence in Presences)
            {
                if (presence.Value.User.Id == pr.User.Id) continue;
                yield return presence.Value.User.Id;
            }
        }

        public static void BeginPresence(Presence presence)
        {
            // TODO: Add total playtime.
            //presence.BeginSeason = DateTime.UtcNow;
            if (presence == null) return;
            presence.LastRequest.Start();
            Presences.Add(presence.Token, presence);
            LChannels.AddChannel(new Channel(presence.User.Username, "", null, presence));
        }

        public static void EndPresence(Presence pr, bool forceful)
        {
            if (forceful && Presences.ContainsKey(pr.Token))
            {
                pr.Stream.Close();
                pr.LastRequest.Stop();
                LChannels.RemoveChannel(pr.PrivateChannel);

                foreach (PacketStream str in pr.JoinedStreams)
                    str.Left(pr);

                Handlers.ExecuteHandler(HandlerTypes.ClientStopSpectating, pr);
                Handlers.ExecuteHandler(HandlerTypes.ClientLobbyPart, pr);
                Handlers.ExecuteHandler(HandlerTypes.ClientMatchPart, pr);

                Presences.Remove(pr.Token);
                return;
            }

            pr.IsLastRequest = true;
        }

        public static void TimeoutCheck() => new Thread(() =>
        {
            while (true)
            {
                try
                {
                    foreach (KeyValuePair<string, Presence> pr in Presences)
                        pr.Value.TimeoutCheck();

                    if (Presences == null) break;
                } catch
                {
                    // Do not EVER let the TimeoutCheck Crash. else we have a Memory Leak.
                }

                Thread.Sleep(1000); // wait a second. we don't want high cpu usage.
            }
        }).Start();
    }

    public class Presence : IComparable
    {
        //public bool Disconnected;

        // ReSharper disable once CollectionNeverUpdated.Global
        public readonly List<PacketStream> JoinedStreams = new List<PacketStream>();
        public readonly Stopwatch LastRequest;

        public bool BlockNonFriendDm;
        public int ClientPermissions;

        public CountryIds CountryId;

        //public DateTime BeginSeason;
        public bool IsLastRequest;
        public MultiplayerRoom JoinedRoom;
        public double Lat;
        public LeaderboardRx LeaderboardRx;

        //public UserStats Stats;
        public LeaderboardStd LeaderboardStd;
        public LeaderboardTouch LeaderboardTouch;
        public double Lon;

        public bool Relax;
        public SpectatorStream Spectator;

        public UserStatus
            Status = new UserStatus {BeatmapChecksum = "", StatusText = ""}; // Predefined strings to prevent Issues.

        public MStreamWriter Stream;
        public byte Timezone;
        public bool Touch;

        public Users User;

        public Presence()
        {
            LastRequest = new Stopwatch();
            Token = Guid.NewGuid().ToString();
            MemoryStream str = new MemoryStream();
            Stream = new MStreamWriter(str);
        }

        public string Token { get; }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public uint Rank => 0;

        public Channel PrivateChannel => LChannels.GetChannel(User.Username);

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof (Presence)) return -1;
            if (!(obj is Presence pr)) return -1;
            return pr.Token == Token ? 0 : 1;
        }

        protected bool Equals(Presence pr) => Token == pr.Token;

        public MemoryStream GetOutput(bool reset = true)
        {
            MemoryStream copy = new MemoryStream();
            long pos = Stream.BaseStream.Position;

            Stream.BaseStream.Position = 0;
            Stream.BaseStream.CopyTo(copy);
            Stream.BaseStream.Position = pos;
            LastRequest.Restart();

            if (!reset) return copy;

            using (Stream)
                // Dispose after chaning stream.
                Stream = new MStreamWriter(new MemoryStream());

            return copy;
        }

        public void TimeoutCheck()
        {
            if (!(LastRequest.Elapsed.TotalSeconds > 30)) return;
            Handlers.ExecuteHandler(HandlerTypes.ClientExit, this, ErrorStates.Ok);
            Handlers.ExecuteHandler(HandlerTypes.ClientLobbyPart, this);
            Handlers.ExecuteHandler(HandlerTypes.ClientMatchPart, this);
            LPresences.EndPresence(this, true);
        }

        public void Write(IPacket p)
        {
            if (!Stream.BaseStream.CanWrite) return;
            if (p != null) Stream?.Write(p);
        }
    }
}
