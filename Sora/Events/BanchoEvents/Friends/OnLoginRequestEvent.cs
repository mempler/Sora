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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MaxMind.GeoIP2.Responses;
using Sora.Attributes;
using Sora.Database;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;
using LeaderboardRx = Sora.Database.Models.LeaderboardRx;
using LeaderboardStd = Sora.Database.Models.LeaderboardStd;
using Localisation = Sora.Helpers.Localisation;
using Logger = Sora.Helpers.Logger;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using PlayMode = Sora.Enums.PlayMode;
using Privileges = Sora.Enums.Privileges;
using Users = Sora.Database.Models.Users;

namespace Sora.Events.BanchoEvents.Friends
{
    [EventClass]
    public class OnLoginRequestEvent
    {
        private readonly SoraDbContextFactory _factory;
        private readonly Config _cfg;
        private readonly PresenceService _pcs;
        private readonly PacketStreamService _ps;
        private readonly ChannelService _cs;

        public OnLoginRequestEvent(SoraDbContextFactory factory, Config cfg, PresenceService pcs, PacketStreamService ps, ChannelService cs)
        {
            _factory = factory;
            _cfg = cfg;
            _pcs = pcs;
            _ps = ps;
            _cs = cs;
        }

        private static List<double> sw_m = new List<double>();
        private static Stopwatch sw = new Stopwatch();
        private static object Locker = new object();
        private static StreamWriter fs;
        
        [Event(EventType.BanchoLoginRequest)]
        public void OnLoginRequest(BanchoLoginRequestArgs args)
        {
            if (fs == null)
                fs = File.AppendText("out.data");
            if (sw_m.Count > 20)
            {
                lock (Locker)
                {
                    Logger.Info();
                    lock (Logger.Locker)
                        foreach (double a in sw_m)
                            fs.Write($"{a}\n");
                    fs.Flush();
                    fs.Close();
                    Logger.Fatal("Done!");
                }

                return;
            }

            sw.Start();
            try
            {
                Login loginData = LoginParser.ParseLogin(args.Reader);
                if (loginData == null)
                {
                    Exception(args.Writer);
                    return;
                }

                Users user = Users.GetUser(_factory, loginData.Username);
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

                args.pr.User = user;

                if (args.IPAddress != "127.0.0.1" && args.IPAddress != "0.0.0.0")
                {
                    CityResponse data = Localisation.GetData(args.IPAddress);
                    args.pr.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);
                    if (data.Location.Longitude != null) args.pr.Lon = (double) data.Location.Longitude;
                    if (data.Location.Latitude != null) args.pr.Lat  = (double) data.Location.Latitude;
                }

                args.pr.LeaderboardStd   = LeaderboardStd.GetLeaderboard(_factory, args.pr.User);
                args.pr.LeaderboardRx    = LeaderboardRx.GetLeaderboard(_factory, args.pr.User);

                args.pr.Timezone         = loginData.Timezone;
                args.pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                args.pr.Status.BeatmapId = 0;
                args.pr.Status.StatusText = "";
                args.pr.Status.CurrentMods = 0;
                args.pr.Status.BeatmapChecksum = "";
                args.pr.Status.Playmode = PlayMode.Osu;
                args.pr.Status.Status   = Status.Idle;
                
                _pcs.BeginPresence(args.pr);

                args.pr.Rank = args.pr.LeaderboardStd.GetPosition(_factory, PlayMode.Osu);

                Success(args.Writer, user.Id);
                args.Writer.Write(new ProtocolNegotiation());
                args.Writer.Write(new UserPresence(args.pr));
                args.Writer.Write(new HandleUpdate(args.pr));
                if (_cfg.Server.FreeDirect)
                    args.Writer.Write(new LoginPermission(LoginPermissions.User | LoginPermissions.Supporter));

                args.Writer.Write(new FriendsList(Database.Models.Friends.GetFriends(_factory, args.pr.User.Id).ToList()));
                args.Writer.Write(new PresenceBundle(_pcs.GetUserIds(args.pr).ToList()));
                foreach (Presence opr in _pcs.AllPresences)
                {
                    args.Writer.Write(new PresenceSingle(opr.User.Id));
                    args.Writer.Write(new UserPresence(opr));
                    args.Writer.Write(new HandleUpdate(opr));
                }

                foreach (Channel chanAuto in _cs.ChannelsAutoJoin)
                {
                    if (chanAuto.AdminOnly && args.pr.User.HasPrivileges(Privileges.Admin))
                        args.Writer.Write(new ChannelAvailableAutojoin(chanAuto));
                    else if (!chanAuto.AdminOnly)
                        args.Writer.Write(new ChannelAvailableAutojoin(chanAuto));

                    if (chanAuto.JoinChannel(args.pr))
                        args.Writer.Write(new ChannelJoinSuccess(chanAuto));
                    else
                        args.Writer.Write(new ChannelRevoked(chanAuto));
                }

                foreach (KeyValuePair<string, Channel> chan in _cs.Channels)
                    if (chan.Value.AdminOnly && args.pr.User.HasPrivileges(Privileges.Admin))
                        args.Writer.Write(new ChannelAvailable(chan.Value));
                    else if (!chan.Value.AdminOnly)
                        args.Writer.Write(new ChannelAvailable(chan.Value));

                PacketStream stream = _ps.GetStream("main");
                if (stream == null)
                {
                    Exception(args.Writer);
                    return;
                }
                
                stream.Broadcast(new PresenceSingle(args.pr.User.Id));
                stream.Broadcast(new UserPresence(args.pr));
                stream.Broadcast(new HandleUpdate(args.pr));
                stream.Join(args.pr);

                args.Writer.Write(new UserPresence(args.pr));
                args.Writer.Write(new HandleUpdate(args.pr));
                
                //Logger.Info("%#F94848%" + args.pr.User.Username, "%#B342F4%(", args.pr.User.Id, "%#B342F4%) %#FFFFFF%has logged in!");
            }
            catch (Exception ex)
            {
                //Logger.Err(ex);
                Exception(args.Writer);
            }


            sw.Stop();
            sw_m.Add(sw.Elapsed.TotalMilliseconds);
            Logger.Info("CUR:", sw.Elapsed.TotalMilliseconds);
            sw.Reset();
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