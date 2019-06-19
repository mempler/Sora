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
using System.Runtime.InteropServices;
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
        private readonly Cache _cache;

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

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        private struct CachableUser
        {
            public int Id;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Username;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Email;
            
            public int Privil;
            public ulong Achievements;
            
            public byte CountryId;
            public double lon;
            public double lat;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        private struct CachableLeaderboard
        {
            public int Id;
            public ulong RankedScoreOsu;
            public ulong RankedScoreTaiko;
            public ulong RankedScoreCtb;
            public ulong RankedScoreMania;
            public ulong TotalScoreOsu;
            public ulong TotalScoreTaiko;
            public ulong TotalScoreCtb;
            public ulong TotalScoreMania;
            public ulong Count300Osu;
            public ulong Count300Taiko;
            public ulong Count300Ctb;
            public ulong Count300Mania;
            public ulong Count100Osu;
            public ulong Count100Taiko;
            public ulong Count100Ctb;
            public ulong Count100Mania;
            public ulong Count50Osu;
            public ulong Count50Taiko;
            public ulong Count50Ctb;
            public ulong Count50Mania;
            public ulong CountMissOsu;
            public ulong CountMissTaiko;
            public ulong CountMissCtb;
            public ulong CountMissMania;
            public ulong PlayCountOsu;
            public ulong PlayCountTaiko;
            public ulong PlayCountCtb;
            public ulong PlayCountMania;
            public double PerformancePointsOsu;
            public double PerformancePointsTaiko;
            public double PerformancePointsCtb;
            public double PerformancePointsMania;

            public uint Rank;
        }

        [Event(EventType.BanchoLoginRequest)]
        public void OnLoginRequest(BanchoLoginRequestArgs args)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                
                Login loginData = LoginParser.ParseLogin(args.Reader);
                if (loginData == null)
                {
                    Exception(args.Writer);
                    return;
                }

                string cacheKey = $"sora:user:{loginData.GetHashCode()}";

                Presence presence = _cache.Get<Presence>(cacheKey);
                if (presence == null)
                {
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

                    if (args.IPAddress != "127.0.0.1" && args.IPAddress != "0.0.0.0")
                    {
                        CityResponse data = Localisation.GetData(args.IPAddress);

                        args.pr.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);

                        args.pr.Lon = data.Location.Longitude ?? 0;
                        args.pr.Lat = data.Location.Latitude ?? 0;
                    }

                    args.pr.User = user;

                    args.pr.LeaderboardRx  = LeaderboardRx.GetLeaderboard(_factory, args.pr.User);
                    args.pr.LeaderboardStd = LeaderboardStd.GetLeaderboard(_factory, args.pr.User);

                    args.pr.Rank = args.pr.LeaderboardStd.GetPosition(_factory, PlayMode.Osu);

                    args.pr.Timezone         = loginData.Timezone;
                    args.pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                    args.pr.Status.BeatmapId       = 0;
                    args.pr.Status.StatusText      = "";
                    args.pr.Status.CurrentMods     = 0;
                    args.pr.Status.BeatmapChecksum = "";
                    args.pr.Status.Playmode        = PlayMode.Osu;
                    args.pr.Status.Status          = Status.Idle;

                    args.pr.Rank = args.pr.LeaderboardStd.GetPosition(_factory, PlayMode.Osu);
                    
                    _cache.Set(cacheKey, args.pr, TimeSpan.FromMinutes(30));
                }
                else
                {
                    string t = args.pr.Token;
                    args.pr = presence;
                    args.pr.Token = t;
                }

                _pcs.BeginPresence(args.pr);
                
                Success(args.Writer, args.pr.User.Id);

                args.pr.Write(new ProtocolNegotiation());
                args.pr.Write(new UserPresence(args.pr));
                args.pr.Write(new HandleUpdate(args.pr));

                if ((args.pr.ClientPermissions & LoginPermissions.Supporter) == 0)
                {
                    if (_cfg.Server.FreeDirect)
                        args.pr.Write(new LoginPermission(LoginPermissions.User | LoginPermissions.Supporter));
                }
                else
                    args.pr.Write(new LoginPermission(args.pr.ClientPermissions));

                args.pr.Write(new FriendsList(Database.Models.Friends.GetFriends(_factory, args.pr.User.Id).ToList()));
                args.pr.Write(new PresenceBundle(_pcs.GetUserIds(args.pr).ToList()));
                foreach (Presence opr in _pcs.AllPresences)
                {
                    args.pr.Write(new PresenceSingle(opr.User.Id));
                    args.pr.Write(new UserPresence(opr));
                    args.pr.Write(new HandleUpdate(opr));
                }

                foreach (Channel chanAuto in _cs.ChannelsAutoJoin)
                {
                    if (chanAuto.AdminOnly && args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr.Write(new ChannelAvailableAutojoin(chanAuto));
                    else if (!chanAuto.AdminOnly)
                        args.pr.Write(new ChannelAvailableAutojoin(chanAuto));

                    if (chanAuto.JoinChannel(args.pr))
                        args.pr.Write(new ChannelJoinSuccess(chanAuto));
                    else
                        args.pr.Write(new ChannelRevoked(chanAuto));
                }

                foreach ((string _, Channel value) in _cs.Channels)
                    if (value.AdminOnly && args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr.Write(new ChannelAvailable(value));
                    else if (!value.AdminOnly)
                        args.pr.Write(new ChannelAvailable(value));
                
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
                
                args.pr
                    .GetOutput()
                    .WriteTo(args.Writer.BaseStream);

                sw.Stop();
                Logger.Info("MS: ", sw.Elapsed.TotalMilliseconds);

                Logger.Info("%#F94848%" + args.pr.User.Username, "%#B342F4%(", args.pr.User.Id, "%#B342F4%) %#FFFFFF%has logged in!");
            }
            catch (Exception ex)
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