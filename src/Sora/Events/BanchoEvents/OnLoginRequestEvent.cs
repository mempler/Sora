#region LICENSE

/*
    olSora - A Modular Bancho written in C#
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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sora.Attributes;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework;
using Sora.Framework.Allocation;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Events.BanchoEvents
{
    [EventClass]
    public class OnLoginRequestEvent
    {
        private readonly Cache _cache;
        private readonly ILogger<OnLoginRequestEvent> _logger;
        private readonly Config _cfg;
        private readonly ChannelService _cs;
        private readonly SoraDbContextFactory _factory;
        private PresenceService _pcs;

        public OnLoginRequestEvent(SoraDbContextFactory factory,
            Config cfg,
            PresenceService pcs,
            ChannelService cs,
            Cache cache,
            ILogger<OnLoginRequestEvent> logger)
        {
            _factory = factory;
            _cfg = cfg;
            _pcs = pcs;
            _cs = cs;
            _cache = cache;
            _logger = logger;
        }

        [Event(EventType.BanchoLoginRequest)]
        public async Task OnLoginRequest(BanchoLoginRequestArgs args)
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

                if (!_cache.TryGet(cacheKey, out Presence presence))
                {
                    var dbUser = await DBUser.GetDBUser(_factory, loginData.Username);
                    var user = dbUser?.ToUser();
                    if (dbUser == null)
                    {
                        LoginFailed(args.Writer);
                        return;
                    }

                    if (!dbUser.IsPassword(loginData.Password))
                    {
                        _logger.Log(LogLevel.Warning, $"{LCOL.RED}{dbUser.UserName} " +
                                                          $"{LCOL.PURPLE}({user.Id}) " +
                                                          $"{LCOL.RED}Failed {LCOL.WHITE}to Login!");
                        LoginFailed(args.Writer);
                        return;
                    }

                    if (args.IPAddress != "127.0.0.1" && args.IPAddress != "0.0.0.0")
                    {
                        var data = Localisation.GetData(args.IPAddress);

                        args.pr.Info.Longitude = data.Location.Longitude ?? 0;
                        args.pr.Info.Latitude = data.Location.Latitude ?? 0;
                        args.pr.Info.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);
                    }

                    args.pr.User = user;
                    
                    args.pr.Info.TimeZone = (byte) loginData.Timezone;

                    var lb = await DBLeaderboard.GetLeaderboardAsync(_factory, dbUser);

                    args.pr["LB"] = lb;

                    args.pr.Stats.TotalScore = lb.TotalScoreOsu;
                    args.pr.Stats.RankedScore = lb.RankedScoreOsu;
                    args.pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsOsu;
                    args.pr.Stats.PlayCount = (uint) lb.PlayCountOsu;
                    args.pr.Stats.Accuracy = (float) lb.GetAccuracy(_factory, PlayMode.Osu);
                    args.pr.Stats.Position = lb.GetPosition(_factory, PlayMode.Osu);

                    //args.pr["BLOCK_NON_FRIENDS_DM"] = loginData.BlockNonFriendDMs;
                    
                    _cache.Set(cacheKey, args.pr, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var t = args.pr.Token;
                    args.pr = presence;
                    args.pr.Token = t;
                }

                if (_pcs.TryGet(args.pr.User.Id, out var oldPresence)) {
                    oldPresence.ActiveMatch?.Leave(args.pr);
                    oldPresence.Spectator?.Leave(args.pr);
                    _pcs.Leave(oldPresence);
                }

                _pcs.Join(args.pr);

                Success(args.Writer, args.pr.User.Id);

                args.pr.Push(new ProtocolNegotiation());
                args.pr.Push(new UserPresence(args.pr));

                // args.pr["ACCURACY"] = args.pr.Get<LeaderboardStd>("LB_STD").GetAccuracy(_factory.Get(), PlayMode.Osu);
                
                args.pr.Push(new HandleUpdate(args.pr));

                args.pr.Info.ClientPermission = LoginPermissions.User;

                if (args.pr.User.Permissions == Permission.GROUP_DONATOR)
                    args.pr.Info.ClientPermission |= LoginPermissions.Supporter;
                if (args.pr.User.Permissions == Permission.GROUP_ADMIN)
                    args.pr.Info.ClientPermission |= LoginPermissions.BAT | LoginPermissions.Administrator | LoginPermissions.Moderator;
                if (args.pr.User.Permissions == Permission.GROUP_DEVELOPER)
                    args.pr.Info.ClientPermission |= LoginPermissions.Developer;

                if (!args.pr.User.Permissions.HasPermission(Permission.GROUP_DONATOR))
                {
                    if (_cfg.Server.FreeDirect)
                        args.pr.Push(new LoginPermission(LoginPermissions.User | LoginPermissions.Supporter |
                                                         args.pr.Info.ClientPermission));
                }
                else
                {
                    args.pr.Push(new LoginPermission(args.pr.Info.ClientPermission));
                }

                args.pr.Push(new FriendsList(DBFriend.GetFriends(_factory, args.pr.User.Id).ToList()));
                args.pr.Push(new PresenceBundle(_pcs.GetUserIds(args.pr).ToList()));
                
                foreach (var chanAuto in _cs.ChannelsAutoJoin)
                {
                    if ((chanAuto.Status & ChannelStatus.AdminOnly) != 0 &&
                        args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr.Push(new ChannelAvailableAutojoin(chanAuto));
                    else if ((chanAuto.Status & ChannelStatus.AdminOnly) == 0)
                        args.pr.Push(new ChannelAvailableAutojoin(chanAuto));

                    args.pr.Push(new ChannelJoinSuccess(chanAuto));
                    chanAuto.Join(args.pr);
                }

                foreach (var channel in _cs.Channels)
                    if ((channel.Status & ChannelStatus.AdminOnly) != 0 &&
                        args.pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.pr.Push(new ChannelAvailable(channel));
                    else if ((channel.Status & ChannelStatus.AdminOnly) == 0)
                        args.pr.Push(new ChannelAvailable(channel));
                
                _pcs.Push(new PresenceSingle(args.pr.User.Id));
                _pcs.Push(new UserPresence(args.pr));
                _pcs.Push(new HandleUpdate(args.pr));
                _pcs.Join(args.pr);

                args.pr.WritePackets(args.Writer.BaseStream);

                sw.Stop();
                _logger.Log(LogLevel.Debug, "Login Time:\nMS: ", sw.Elapsed.TotalMilliseconds);

                _logger.Log(LogLevel.Information,
                    $"{LCOL.RED}{args.pr.User.UserName} {LCOL.PURPLE}({args.pr.User.Id}) {LCOL.WHITE}has logged in!"
                );

                args.pr["LAST_PONG"] = DateTime.Now;
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
