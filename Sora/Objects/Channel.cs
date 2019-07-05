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
using System.Threading;
using Sora.Allocation;
using Sora.Helpers;
using Sora.Packets.Server;

namespace Sora.Objects
{
    public class Channel : DynamicValues
    {
        private readonly Mutex _mut = new Mutex();
        private readonly List<Presence> _presences = new List<Presence>(); // should be { get; } maybe ?

        private int _userCount = -1;

        public Channel(
            string channelName, string channelTopic = "",
            PacketStream boundStream = null, Presence boundPresence = null,
            bool readOnly = false, bool adminOnly = false, bool autoJoin = false)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            BoundStream = boundStream;
            BoundPresence = boundPresence;
            ReadOnly = readOnly;
            AdminOnly = adminOnly;
            AutoJoin = autoJoin;
        }

        public string ChannelName { get; }
        public string ChannelTopic { get; }
        public PacketStream BoundStream { get; }
        public Presence BoundPresence { get; private set; }
        public bool ReadOnly { get; set; }
        public bool AdminOnly { get; }
        public bool AutoJoin { get; }
        public List<Presence> PresenceList => _presences;

        public int UserCount
        {
            get
            {
                if (_userCount > -1)
                    return _userCount;
                if (_presences == null)
                    return 0;

                _mut.WaitOne();
                var c = _presences.Count;
                _mut.ReleaseMutex();

                return c;
            }
            set => _userCount = value;
        }

        public bool JoinChannel(Presence pr)
        {
            if (AdminOnly &&
                pr.User.Permissions == Permission.ADMIN_CHANNEL)
            {
                _mut.WaitOne();
                _presences.Remove(pr);
                _presences.Add(pr);
                _mut.ReleaseMutex();
                
                return true;
            }

            if (AdminOnly)
                return false;

            _mut.WaitOne();
            _presences.Remove(pr);
            _presences.Add(pr);
            _mut.ReleaseMutex();

            return true;
        }

        public void LeaveChannel(Presence pr)
        {
            try
            {
                lock (_presences)
                {
                    _presences.Remove(pr);
                }
            } catch
            {
                // Ignored
            }
        }

        public void WriteMessage(Presence pr, string message, bool skipReadonly = false)
        {
            if (!skipReadonly && pr.User.Id != 100)
                if (ReadOnly)
                    return;

            Logger.Info(
                $"{L_COL.RED}{pr.User.Username}",
                $"{L_COL.PURPLE}( {pr.User.Id} )",
                $"{L_COL.YELLOW}{ChannelName}",
                $"{L_COL.WHITE}=>", message.Replace("\n", " ")
            );

            if (BoundStream == null && BoundPresence != null)
            {
                BoundPresence +=
                    new SendIrcMessage(
                        new MessageStruct
                        {
                            Username = pr.User.Username,
                            ChannelTarget = pr.User.Username,
                            Message = message,
                            SenderId = pr.User.Id
                        }
                    );
                return;
            }

            BoundStream?.Broadcast(
                new SendIrcMessage(
                    new MessageStruct
                    {
                        Username = pr.User.Username,
                        ChannelTarget = ChannelName,
                        Message = message,
                        SenderId = pr.User.Id
                    }
                ), pr
            );
        }

        public override string ToString()
            => $"Channel: {ChannelName} ChannelTopic: {ChannelTopic} BoundStream: {BoundStream?.StreamName} ChannelOwner: {BoundPresence?.User?.Username}";
    }
}
