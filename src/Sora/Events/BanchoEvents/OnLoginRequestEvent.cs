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
        private readonly SoraDbContext _ctx;
        private PresenceService _pcs;

        public OnLoginRequestEvent(SoraDbContext ctx,
                                   Config cfg,
                                   PresenceService pcs,
                                   ChannelService cs,
                                   Cache cache,
                                   ILogger<OnLoginRequestEvent> logger)
        {
            _ctx = ctx;
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
                    var dbUser = await DbUser.GetDbUser(_ctx, loginData.Username);
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

                    if (args.IpAddress != "127.0.0.1" && args.IpAddress != "0.0.0.0")
                    {
                        var data = Localisation.GetData(args.IpAddress);

                        args.Pr.Info.Longitude = data.Location.Longitude ?? 0;
                        args.Pr.Info.Latitude = data.Location.Latitude ?? 0;
                        args.Pr.Info.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);
                    }

                    args.Pr.User = user;
                    
                    args.Pr.Info.TimeZone = (byte) loginData.Timezone;

                    var lb = await DbLeaderboard.GetLeaderboardAsync(_ctx, dbUser);

                    args.Pr["LB"] = lb;

                    args.Pr.Stats.TotalScore = lb.TotalScoreOsu;
                    args.Pr.Stats.RankedScore = lb.RankedScoreOsu;
                    args.Pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsOsu;
                    args.Pr.Stats.PlayCount = (uint) lb.PlayCountOsu;
                    args.Pr.Stats.Accuracy = (float) lb.GetAccuracy(_ctx, PlayMode.Osu);
                    args.Pr.Stats.Position = lb.GetPosition(_ctx, PlayMode.Osu);

                    //args.pr["BLOCK_NON_FRIENDS_DM"] = loginData.BlockNonFriendDMs;
                    
                    _cache.Set(cacheKey, args.Pr, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var t = args.Pr.Token;
                    args.Pr = presence;
                    args.Pr.Token = t;
                }

                if (_pcs.TryGet(args.Pr.User.Id, out var oldPresence)) {
                    oldPresence.ActiveMatch?.Leave(args.Pr);
                    oldPresence.Spectator?.Leave(args.Pr);
                    _pcs.Leave(oldPresence);
                }

                _pcs.Join(args.Pr);

                Success(args.Writer, args.Pr.User.Id);

                args.Pr.Push(new ProtocolNegotiation());
                args.Pr.Push(new UserPresence(args.Pr));

                args.Pr.Info.ClientPermission = LoginPermissions.User;

                if (args.Pr.User.Permissions == Permission.GROUP_DONATOR)
                    args.Pr.Info.ClientPermission |= LoginPermissions.Supporter;
                if (args.Pr.User.Permissions == Permission.GROUP_ADMIN)
                    args.Pr.Info.ClientPermission |= LoginPermissions.BAT | LoginPermissions.Administrator | LoginPermissions.Moderator;
                if (args.Pr.User.Permissions == Permission.GROUP_DEVELOPER)
                    args.Pr.Info.ClientPermission |= LoginPermissions.Developer;

                if (!args.Pr.User.Permissions.HasPermission(Permission.GROUP_DONATOR))
                {
                    if (_cfg.Server.FreeDirect)
                        args.Pr.Push(new LoginPermission(LoginPermissions.User | LoginPermissions.Supporter |
                                                         args.Pr.Info.ClientPermission));
                }
                else
                {
                    args.Pr.Push(new LoginPermission(args.Pr.Info.ClientPermission));
                }

                args.Pr.Push(new FriendsList(DbFriend.GetFriends(_ctx, args.Pr.User.Id).ToList()));
                args.Pr.Push(new PresenceBundle(_pcs.GetUserIds(args.Pr).ToList()));
                
                foreach (var chanAuto in _cs.ChannelsAutoJoin)
                {
                    if ((chanAuto.Status & ChannelStatus.AdminOnly) != 0 &&
                        args.Pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.Pr.Push(new ChannelAvailableAutojoin(chanAuto));
                    else if ((chanAuto.Status & ChannelStatus.AdminOnly) == 0)
                        args.Pr.Push(new ChannelAvailableAutojoin(chanAuto));

                    args.Pr.Push(new ChannelJoinSuccess(chanAuto));
                    chanAuto.Join(args.Pr);
                }

                foreach (var channel in _cs.Channels)
                    if ((channel.Status & ChannelStatus.AdminOnly) != 0 &&
                        args.Pr.User.Permissions == Permission.ADMIN_CHANNEL)
                        args.Pr.Push(new ChannelAvailable(channel));
                    else if ((channel.Status & ChannelStatus.AdminOnly) == 0)
                        args.Pr.Push(new ChannelAvailable(channel));
                
                _pcs.Push(new PresenceSingle(args.Pr.User.Id));
                _pcs.Join(args.Pr);

                args.Pr.WritePackets(args.Writer.BaseStream);

                sw.Stop();
                _logger.Log(LogLevel.Debug, "Login Time:\nMS: ", sw.Elapsed.TotalMilliseconds);

                _logger.Log(LogLevel.Information,
                    $"{LCOL.RED}{args.Pr.User.UserName} {LCOL.PURPLE}( {args.Pr.User.Id} ) {LCOL.WHITE}has logged in!"
                );

                args.Pr["LAST_PONG"] = DateTime.Now;
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
