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
using Privileges = Sora.Enums.Privileges;
using Users = Sora.Database.Models.Users;

namespace Sora.Events.BanchoEvents.Friends
{
    [EventClass]
    public class OnLoginRequestEvent
    {
        private readonly SoraDbContextFactory _factory;
        private readonly Config _cfg;
        private readonly Cache _cache;
        private readonly PresenceService _pcs;
        private readonly PacketStreamService _ps;
        private readonly ChannelService _cs;

        public OnLoginRequestEvent(SoraDbContextFactory factory, Config cfg, Cache cache, PresenceService pcs, PacketStreamService ps, ChannelService cs)
        {
            _factory = factory;
            _cfg = cfg;
            _cache = cache;
            _pcs = pcs;
            _ps = ps;
            _cs = cs;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        private struct CachableUser
        {
            public int Id;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Username;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
            public string Password;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
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
                CachableUser cachedUser = _cache.GetCachedStruct<CachableUser>(cacheKey);
                Users user;
                
                if (cachedUser.Username == null) {
                    user = Users.GetUser(_factory, loginData.Username);
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

                    cachedUser = new CachableUser
                    {
                        Id           = user.Id,
                        Password     = user.Password,
                        Username     = user.Username,
                        Achievements = user.Achievements,
                        Email        = user.Email,
                        Privil       = (int) user.Privileges
                    };

                    if (args.IPAddress != "127.0.0.1" && args.IPAddress != "0.0.0.0")
                    {
                        CityResponse data = Localisation.GetData(args.IPAddress);

                        cachedUser.CountryId = (byte) Localisation.StringToCountryId(data.Country.IsoCode);
                        if (data.Location.Longitude != null)
                            cachedUser.lon = (double) data.Location.Longitude;
                        if (data.Location.Latitude != null)
                            cachedUser.lat = (double) data.Location.Latitude;
                    }
                    
                    args.pr.CountryId = (CountryIds) cachedUser.CountryId;
                    args.pr.Lon = cachedUser.lon;
                    args.pr.Lat = cachedUser.lat;
                    
                    _cache.CacheStruct($"sora:user:{loginData.GetHashCode()}", ref cachedUser);
                }
                else
                {
                    user = new Users
                    {
                        Username = cachedUser.Username,
                        Password = cachedUser.Password,
                        Email = cachedUser.Email,
                        Achievements = cachedUser.Achievements,
                        Id = cachedUser.Id,
                        Privileges = (Privileges) cachedUser.Privil
                    };

                    args.pr.CountryId = (CountryIds) cachedUser.CountryId;
                    args.pr.Lon       = cachedUser.lon;
                    args.pr.Lat       = cachedUser.lat;
                }

                args.pr.User = user;

                CachableLeaderboard cachedLBRX = _cache.GetCachedStruct<CachableLeaderboard>(cacheKey + ":LBRX");

                if (cachedLBRX.Id == 0)
                {
                    LeaderboardRx lbrx = LeaderboardRx.GetLeaderboard(_factory, args.pr.User);

                    cachedLBRX = new CachableLeaderboard
                    {
                        Id = lbrx.Id,
                        Count50Osu = lbrx.Count50Osu,
                        Count50Taiko = lbrx.Count50Taiko,
                        Count50Ctb = lbrx.Count50Ctb,
                        
                        
                        Count100Osu = lbrx.Count100Osu,
                        Count100Taiko = lbrx.Count100Taiko,
                        Count100Ctb = lbrx.Count100Ctb,

                        Count300Osu   = lbrx.Count300Osu,
                        Count300Taiko = lbrx.Count300Taiko,
                        Count300Ctb   = lbrx.Count300Ctb,

                        CountMissOsu = lbrx.CountMissOsu,
                        CountMissTaiko = lbrx.CountMissTaiko,
                        CountMissCtb   = lbrx.CountMissCtb,
                        
                        PlayCountOsu = lbrx.PlayCountOsu,
                        PlayCountTaiko = lbrx.PlayCountTaiko,
                        PlayCountCtb = lbrx.PlayCountCtb,

                        PerformancePointsOsu   = lbrx.PerformancePointsOsu,
                        PerformancePointsTaiko = lbrx.PerformancePointsTaiko,
                        PerformancePointsCtb   = lbrx.PerformancePointsCtb,
                    };

                    _cache.CacheStruct(cacheKey + ":LBRX", ref cachedLBRX);
                    args.pr.LeaderboardRx = lbrx;
                }
                else
                {
                    args.pr.LeaderboardRx = new LeaderboardRx
                    {
                        Id           = cachedLBRX.Id,
                        Count50Osu   = cachedLBRX.Count50Osu,
                        Count50Taiko = cachedLBRX.Count50Taiko,
                        Count50Ctb   = cachedLBRX.Count50Ctb,

                        Count100Osu   = cachedLBRX.Count100Osu,
                        Count100Taiko = cachedLBRX.Count100Taiko,
                        Count100Ctb   = cachedLBRX.Count100Ctb,

                        Count300Osu   = cachedLBRX.Count300Osu,
                        Count300Taiko = cachedLBRX.Count300Taiko,
                        Count300Ctb   = cachedLBRX.Count300Ctb,

                        CountMissOsu   = cachedLBRX.CountMissOsu,
                        CountMissTaiko = cachedLBRX.CountMissTaiko,
                        CountMissCtb   = cachedLBRX.CountMissCtb,

                        PlayCountOsu   = cachedLBRX.PlayCountOsu,
                        PlayCountTaiko = cachedLBRX.PlayCountTaiko,
                        PlayCountCtb   = cachedLBRX.PlayCountCtb,

                        PerformancePointsOsu   = cachedLBRX.PerformancePointsOsu,
                        PerformancePointsTaiko = cachedLBRX.PerformancePointsTaiko,
                        PerformancePointsCtb   = cachedLBRX.PerformancePointsCtb
                    };
                }
                
                CachableLeaderboard cachedLBSTD = _cache.GetCachedStruct<CachableLeaderboard>(cacheKey + ":LBSTD");
                
                if (cachedLBSTD.Id == 0)
                {
                    LeaderboardStd lbstd = LeaderboardStd.GetLeaderboard(_factory, args.pr.User);

                    cachedLBSTD = new CachableLeaderboard
                    {
                        Id = lbstd.Id,
                        Count50Osu = lbstd.Count50Osu,
                        Count50Taiko = lbstd.Count50Taiko,
                        Count50Ctb = lbstd.Count50Ctb,
                        Count50Mania = lbstd.Count50Mania,

                        Count100Osu = lbstd.Count100Osu,
                        Count100Taiko = lbstd.Count100Taiko,
                        Count100Ctb = lbstd.Count100Ctb,
                        Count100Mania = lbstd.Count100Mania,

                        Count300Osu   = lbstd.Count300Osu,
                        Count300Taiko = lbstd.Count300Taiko,
                        Count300Ctb   = lbstd.Count300Ctb,
                        Count300Mania = lbstd.Count300Mania,

                        CountMissOsu = lbstd.CountMissOsu,
                        CountMissTaiko = lbstd.CountMissTaiko,
                        CountMissCtb   = lbstd.CountMissCtb,
                        CountMissMania = lbstd.CountMissMania,
                        
                        PlayCountOsu = lbstd.PlayCountOsu,
                        PlayCountTaiko = lbstd.PlayCountTaiko,
                        PlayCountCtb = lbstd.PlayCountCtb,
                        PlayCountMania = lbstd.PlayCountMania,

                        PerformancePointsOsu   = lbstd.PerformancePointsOsu,
                        PerformancePointsTaiko = lbstd.PerformancePointsTaiko,
                        PerformancePointsCtb   = lbstd.PerformancePointsCtb,
                        PerformancePointsMania = lbstd.PerformancePointsMania,
                        Rank = lbstd.GetPosition(_factory, PlayMode.Osu)
                    };

                    _cache.CacheStruct(cacheKey + ":LBSTD", ref cachedLBSTD);

                    args.pr.Rank = cachedLBSTD.Rank;
                    args.pr.LeaderboardStd = lbstd;
                }
                else
                {
                    args.pr.LeaderboardStd = new LeaderboardStd
                    {
                        Id           = cachedLBSTD.Id,
                        Count50Osu   = cachedLBSTD.Count50Osu,
                        Count50Taiko = cachedLBSTD.Count50Taiko,
                        Count50Ctb   = cachedLBSTD.Count50Ctb,
                        Count50Mania = cachedLBSTD.Count50Mania,

                        Count100Osu   = cachedLBSTD.Count100Osu,
                        Count100Taiko = cachedLBSTD.Count100Taiko,
                        Count100Ctb   = cachedLBSTD.Count100Ctb,
                        Count100Mania = cachedLBSTD.Count100Mania,

                        Count300Osu   = cachedLBSTD.Count300Osu,
                        Count300Taiko = cachedLBSTD.Count300Taiko,
                        Count300Ctb   = cachedLBSTD.Count300Ctb,
                        Count300Mania = cachedLBSTD.Count300Mania,

                        CountMissOsu   = cachedLBSTD.CountMissOsu,
                        CountMissTaiko = cachedLBSTD.CountMissTaiko,
                        CountMissCtb   = cachedLBSTD.CountMissCtb,
                        CountMissMania = cachedLBSTD.CountMissMania,

                        PlayCountOsu   = cachedLBSTD.PlayCountOsu,
                        PlayCountTaiko = cachedLBSTD.PlayCountTaiko,
                        PlayCountCtb   = cachedLBSTD.PlayCountCtb,
                        PlayCountMania = cachedLBSTD.PlayCountMania,

                        PerformancePointsOsu   = cachedLBSTD.PerformancePointsOsu,
                        PerformancePointsTaiko = cachedLBSTD.PerformancePointsTaiko,
                        PerformancePointsCtb   = cachedLBSTD.PerformancePointsCtb,
                        PerformancePointsMania = cachedLBSTD.PerformancePointsMania
                    };

                    args.pr.Rank = cachedLBSTD.Rank;
                }
                

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
                args.pr.Write(new ProtocolNegotiation());
                args.pr.Write(new UserPresence(args.pr));
                args.pr.Write(new HandleUpdate(args.pr));
                if ((args.pr.ClientPermissions & LoginPermissions.Supporter) == 0) {
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
                    if (chanAuto.AdminOnly && args.pr.User.HasPrivileges(Privileges.Admin))
                        args.pr.Write(new ChannelAvailableAutojoin(chanAuto));
                    else if (!chanAuto.AdminOnly)
                        args.pr.Write(new ChannelAvailableAutojoin(chanAuto));

                    if (chanAuto.JoinChannel(args.pr))
                        args.pr.Write(new ChannelJoinSuccess(chanAuto));
                    else
                        args.pr.Write(new ChannelRevoked(chanAuto));
                }

                foreach ((string _, Channel value) in _cs.Channels)
                    if (value.AdminOnly && args.pr.User.HasPrivileges(Privileges.Admin))
                        args.pr.Write(new ChannelAvailable(value));
                    else if (!value.AdminOnly)
                        args.pr.Write(new ChannelAvailable(value));

                args.pr.Write(new UserPresence(args.pr));
                args.pr.Write(new HandleUpdate(args.pr));

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