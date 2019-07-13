#region LICENSE

/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

#nullable enable
namespace Sora.Services
{
    public class PresenceService : IEnumerable<Presence>
    {
        private readonly ChannelService _cs;
        private readonly EventManager _ev;

        private readonly Dictionary<string, Presence> _presences;
        public object Locker = new object();

        public PresenceService(ChannelService cs, EventManager ev)
        {
            _cs = cs;
            _ev = ev;
            _presences = new Dictionary<string, Presence>();
        }

        public IEnumerable<Presence> AllPresences
        {
            get
            {
                lock (Locker)
                {
                    return _presences.Select(x => x.Value);
                }
            }
        }

        public int ConnectedPresences
        {
            get
            {
                if (_presences == null)
                    return 0;
                
                lock (Locker)
                {
                    return _presences.Count;
                }
            }
        }

        public Presence? this[string token] => GetPresence(token);

        public Presence? this[int userId] => GetPresence(userId);

        public IEnumerator<Presence> GetEnumerator() => AllPresences.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Presence? GetPresence(string token)
        {
            lock (Locker)
            {
                return _presences.TryGetValue(token, out var pr) ? pr : null;
            }
        }

        public Presence? GetPresence(int userId)
        {
            lock (Locker)
            {
                return _presences.FirstOrDefault(x => x.Value.User.Id == userId).Value;
            }
        }

        [UsedImplicitly]
        public IEnumerable<int> GetUserIds()
        {
            lock (Locker)
            {
                return _presences.Select(x => x.Value.User.Id);
            }
        }

        public IEnumerable<int> GetUserIds(Presence pr)
        {
            lock (Locker)
            {
                return _presences
                       .Where(x => x.Value.Token != pr.Token)
                       .Select(z => z.Value.User.Id);
            }
        }

        public PresenceService BeginPresence(Presence presence)
        {
            if (presence == null)
                return this;

            // TODO: Add total playtime.
            //presence.BeginSeason = DateTime.UtcNow;

            var cl_perms = LoginPermissions.User;
            
            if (presence.User.Permissions == Permission.COLOR_ORANGE)
                cl_perms |= LoginPermissions.Supporter;
            if (presence.User.Permissions == Permission.COLOR_RED)
            {
                if (presence.User.Permissions == Permission.COLOR_ORANGE)
                    cl_perms -= LoginPermissions.Supporter;
                cl_perms |= LoginPermissions.BAT;
            }

            if (presence.User.Permissions == Permission.COLOR_BLUE)
            {
                if (presence.User.Permissions == Permission.COLOR_RED)
                    cl_perms -= LoginPermissions.BAT;
                cl_perms |= LoginPermissions.Developer;
            }

            presence["CL_PERMISSIONS"] = cl_perms;

            presence.LastRequest.Start();
            _cs.AddChannel(new Channel(presence.User.Username, "", null, presence));

            lock (Locker)
            {
                _presences.Add(presence.Token, presence);
            }

            return this;
        }

        public static PresenceService operator +(PresenceService instance, Presence pr) => instance.BeginPresence(pr);

        public static PresenceService operator -(PresenceService instance, Presence pr)
            => instance.EndPresence(pr, true);

        public PresenceService EndPresence(Presence pr, bool forceful)
        {
            bool b;
            lock (Locker)
            {
                b = _presences.ContainsKey(pr.Token);
            }

            if (forceful && b)
            {
                pr.LastRequest.Stop();

                _cs.RemoveChannel(pr.PrivateChannel);

                foreach (var str in pr.JoinedStreams)
                    str.Left(pr);

                lock (Locker)
                {
                    #pragma warning disable 4014
                    _ev.RunEvent(
                        EventType.BanchoExit,
                        new BanchoExitArgs {pr = pr, err = ErrorStates.Ok}
                    );
                    _ev.RunEvent(EventType.BanchoLobbyPart, new BanchoLobbyPartArgs {pr = pr});
                    _ev.RunEvent(EventType.BanchoMatchPart, new BanchoMatchPartArgs {pr = pr});
                    _ev.RunEvent(EventType.BanchoStopSpectating, new BanchoStopSpectatingArgs {pr = pr});
                    #pragma warning restore 4014
                    _presences.Remove(pr.Token);
                }

                return this;
            }

            pr["IS_LAST_REQUEST"] = true;
            return this;
        }

        public void TimeoutCheck()
        {
            new Thread(
                () =>
                {
                    while (true)
                    {
                        try
                        {
                            var toRemove = new List<Presence>();
                            lock (Locker)
                            {
                                foreach ((string _, Presence? value) in _presences)
                                    if ((bool?) value["IRC"] == true)
                                        if (value.TimeoutCheck())
                                            toRemove.Add(value);
                            }

                            foreach (var pr in toRemove)
                                EndPresence(pr, true);
                        } catch
                        {
                            // Do not EVER let the TimeoutCheck Crash. else we have a Memory Leak.
                        }

                        Thread.Sleep(5000); // wait 5 seconds. we don't want high cpu usage.
                    }
                }
            ).Start();
        }
    }
}
