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

using System;
using System.Diagnostics;
using System.Linq;
using Sora.Allocation;
using Sora.Attributes;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents
{
    [EventClass]
    public class OnLoginRequestEvent
    {
        private readonly Cache _cache;
        private readonly Config _cfg;
        private readonly ChannelService _cs;
        private readonly SoraDbContextFactory _factory;
        private readonly PacketStreamService _ps;
        private PresenceService _pcs;

        public OnLoginRequestEvent(SoraDbContextFactory factory,
            Config cfg,
            PresenceService pcs,
            PacketStreamService ps,
            ChannelService cs,
            Cache cache)
        {
            _factory = factory;
            _cfg = cfg;
            _pcs = pcs;
            _ps = ps;
            _cs = cs;
            _cache = cache;
        }

        [Event(EventType.BanchoLoginRequest)]
        public void OnLoginRequest(BanchoLoginRequestArgs args)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var loginData = LoginParser.ParseLogin(args.Reader);
                if (loginData == null)
                {
                    Exception(args.Writer);
                    return;
                }

                var cacheKey = $"sora:user:{loginData.GetHashCode()}";

                var presence = _cache.Get<Presence>(cacheKey);
                if (presence == null)
                {
                    var user = Users.GetUser(_factory, loginData.Username);
                    if (user == null)
                    {
                        LoginFailed(args.Writer);
                        return;
                    }

                    if (!user.IsPassword(loginData.Password))
                    {
                        LoginFailed(args.Writer);
                        return;
                    }

                    if (args.IPAddress != "127.0.0.1" && args.IPAddress != "0.0.0.0")
                    {
                        var data = Localisation.GetData(args.IPAddress);

                        args.pr.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);

                        args.pr.Lon = data.Location.Longitude ?? 0;
                        args.pr.Lat = data.Location.Latitude ?? 0;
                    }

                    args.pr.User = user;

                    args.pr.LeaderboardRx = LeaderboardRx.GetLeaderboard(_factory, args.pr.User);
                    args.pr.LeaderboardStd = LeaderboardStd.GetLeaderboard(_factory, args.pr.User);

                    args.pr.Rank = args.pr.LeaderboardStd.GetPosition(_factory, PlayMode.Osu);

                    args.pr.Timezone = loginData.Timezone;
                    args.pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                    args.pr.Status.BeatmapId = 0;
                    args.pr.Status.StatusText = "";
                    args.pr.Status.CurrentMods = 0;
                    args.pr.Status.BeatmapChecksum = "";
                    args.pr.Status.Playmode = PlayMode.Osu;
                    args.pr.Status.Status = Status.Idle;

                    args.pr.Rank = args.pr.LeaderboardStd.GetPosition(_factory, PlayMode.Osu);

                    _cache.Set(cacheKey, args.pr, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var t = args.pr.Token;
                    args.pr = presence;
                    args.pr.Token = t;
                }

                _pcs += args.pr;

                Success(args.Writer, args.pr.User.Id);

                args.pr += new ProtocolNegotiation();
                args.pr += new UserPresence(args.pr);

                args.pr["ACCURACY"] = args.pr.LeaderboardStd.GetAccuracy(_factory, PlayMode.Osu);
                
                args.pr += new HandleUpdate(args.pr);

                if ((args.pr.ClientPermissions & LoginPermissions.Supporter) == 0)
                {
                    if (_cfg.Server.FreeDirect)
                        args.pr += new LoginPermission(LoginPermissions.User | LoginPermissions.Supporter);
                }
                else
                {
                    args.pr += new LoginPermission(args.pr.ClientPermissions);
                }

                args.pr += new FriendsList(Database.Models.Friends.GetFriends(_factory, args.pr.User.Id).ToList());
                args.pr += new PresenceBundle(_pcs.GetUserIds(args.pr).ToList());
                foreach (var opr in _pcs.AllPresences)
                {
                    args.pr += new PresenceSingle(opr.User.Id);
                    args.pr += new UserPresence(opr);
                    args.pr += new HandleUpdate(opr);
                }

                foreach (var chanAuto in _cs.ChannelsAutoJoin)
                {
                    if (chanAuto.AdminOnly && args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr += new ChannelAvailableAutojoin(chanAuto);
                    else if (!chanAuto.AdminOnly)
                        args.pr += new ChannelAvailableAutojoin(chanAuto);

                    if (chanAuto.JoinChannel(args.pr))
                        args.pr += new ChannelJoinSuccess(chanAuto);
                    else
                        args.pr += new ChannelRevoked(chanAuto);
                }

                foreach ((string _, Channel value) in _cs.Channels)
                    if (value.AdminOnly && args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr += new ChannelAvailable(value);
                    else if (!value.AdminOnly)
                        args.pr += new ChannelAvailable(value);

                var stream = _ps.GetStream("main");
                if (stream == null)
                {
                    Exception(args.Writer);
                    return;
                }

                stream.Broadcast(new PresenceSingle(args.pr.User.Id));
                stream.Broadcast(new UserPresence(args.pr));
                stream.Broadcast(new HandleUpdate(args.pr));
                stream.Join(args.pr);

                args.pr
                    .GetOutput()
                    .WriteTo(args.Writer.BaseStream);

                sw.Stop();
                Logger.Info("MS: ", sw.Elapsed.TotalMilliseconds);

                Logger.Info(
                    "%#F94848%" + args.pr.User.Username, "%#B342F4%(", args.pr.User.Id,
                    "%#B342F4%) %#FFFFFF%has logged in!"
                );
            } catch (Exception ex)
            {
                Logger.Err(ex);
                Exception(args.Writer);
            }
        }

        private void LoginFailed(MStreamWriter dataWriter)
        {
            dataWriter.Write(new LoginResponse(LoginResponses.Failed));
        }

        private void Exception(MStreamWriter dataWriter)
        {
            dataWriter.Write(new LoginResponse(LoginResponses.Exception));
        }

        private void Success(MStreamWriter dataWriter, int userid)
        {
            dataWriter.Write(new LoginResponse((LoginResponses) userid));
        }
    }
}
