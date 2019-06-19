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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Objects;

namespace Sora.Services
{
    public class PresenceService
    {
        private readonly ChannelService _cs;
        private readonly EventManager _ev;
        
        private readonly Dictionary<string, Presence> _presences = new Dictionary<string, Presence>();
        public object Locker = new object();

        public IEnumerable<Presence> AllPresences
        {
            get
            {
                lock (Locker)
                    return _presences.Select(x => x.Value);
            }
        }

        public PresenceService(ChannelService cs, EventManager ev)
        {
            _cs = cs;
            _ev = ev;
        }
        
        public Presence GetPresence(string token)
        {
            lock (Locker)
                return _presences.TryGetValue(token, out Presence pr) ? pr : null;
        }

        public Presence GetPresence(int userid)
        {
            lock (Locker)
                foreach (KeyValuePair<string, Presence> presence in _presences)
                    if (presence.Value.User.Id == userid)
                        return presence.Value;

            return null;
        }

        [UsedImplicitly]
        public IEnumerable<int> GetUserIds()
        {
            lock (Locker)
                return _presences.Select(x => x.Value.User.Id);
        }

        public IEnumerable<int> GetUserIds(Presence pr)
        {
            lock (Locker)
                return _presences
                   .Where(x => x.Value.Token != pr.Token)
                   .Select(z => z.Value.User.Id);
        }

        public void BeginPresence(Presence presence)
        {
            lock (Locker) {
                // TODO: Add total playtime.
                //presence.BeginSeason = DateTime.UtcNow;
                if (presence == null) return;
                
                presence.ClientPermissions |= LoginPermissions.User;
                if (presence.User.HasPrivileges(Privileges.ColorORANGE))
                    presence.ClientPermissions |= LoginPermissions.Supporter;
                if (presence.User.HasPrivileges(Privileges.ColorRED))
                {
                    if (presence.User.HasPrivileges(Privileges.ColorORANGE))
                        presence.ClientPermissions -= LoginPermissions.Supporter;
                    presence.ClientPermissions |= LoginPermissions.BAT;
                }
                if (presence.User.HasPrivileges(Privileges.ColorBLUE)) {
                    if (presence.User.HasPrivileges(Privileges.ColorRED))
                        presence.ClientPermissions -= LoginPermissions.BAT;
                    presence.ClientPermissions |= LoginPermissions.Developer;
                }
                
                presence.LastRequest.Start();
                _presences.Add(presence.Token, presence);
                _cs.AddChannel(new Channel(presence.User.Username, "", null, presence));
            }
        }

        public void EndPresence(Presence pr, bool forceful)
        {
            lock (Locker)
            {
                if (forceful && _presences.ContainsKey(pr.Token))
                {
                    _cs.RemoveChannel(pr.PrivateChannel);

                    foreach (PacketStream str in pr.JoinedStreams)
                        str.Left(pr);

                    #pragma warning disable 4014
                    _ev.RunEvent(EventType.BanchoExit, new BanchoExitArgs {pr = pr, err = ErrorStates.Ok});
                    _ev.RunEvent(EventType.BanchoLobbyPart, new BanchoLobbyPartArgs{ pr = pr});
                    _ev.RunEvent(EventType.BanchoMatchPart, new BanchoMatchPartArgs{ pr = pr});
                    _ev.RunEvent(EventType.BanchoStopSpectating, new BanchoStopSpectatingArgs { pr = pr });
                    #pragma warning restore 4014
                    
                    pr.LastRequest.Stop();
                
                    _presences.Remove(pr.Token);
                    return;
                }
            }

            pr["IS_LAST_REQUEST"] = true;
        }

        public void TimeoutCheck()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        List<Presence> toRemove = new List<Presence>();
                        lock (Locker)
                        {
                            foreach ((string _, Presence value) in _presences)
                                if (!value.BotPresence)
                                    if (value.TimeoutCheck())
                                        toRemove.Add(value);
                        }

                        foreach (Presence pr in toRemove)
                            EndPresence(pr, true);

                        if (_presences == null) break;
                    }
                    catch
                    {
                        // Do not EVER let the TimeoutCheck Crash. else we have a Memory Leak.
                    }

                    Thread.Sleep(1000); // wait a second. we don't want high cpu usage.
                }
            }).Start();
        }
    }
}