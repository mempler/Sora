using System;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Enums;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Server;

namespace Sora.Handler
{
    internal class LoginHandler
    {
        [Handler(HandlerTypes.LoginHandler)]
        public void OnLogin(Req req, Res res)
        {
            var pr = new Presence();
            res.Headers["cho-token"] = pr.Token;
            try
            {
                var loginData = LoginParser.ParseLogin(req.Reader);
                if (loginData == null)
                {
                    Exception(res);
                    return;
                }

                var user = Users.GetUser(Users.GetUserId(loginData.Username));
                if (user == null)
                {
                    LoginFailed(res);
                    return;
                }

                pr.User = user;

                if (req.Ip != "127.0.0.1" && req.Ip != "0.0.0.0")
                {
                    var data = Localisation.GetData(req.Ip);
                    pr.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);
                    if (data.Location.Longitude != null) pr.Lon = (double) data.Location.Longitude;
                    if (data.Location.Latitude != null) pr.Lat = (double) data.Location.Latitude;
                }

                pr.LeaderboardStd = LeaderboardStd.GetLeaderboard(pr.User);
                pr.LeaderboardRx = LeaderboardRx.GetLeaderboard(pr.User);
                pr.LeaderboardTouch = LeaderboardTouch.GetLeaderboard(pr.User);

                pr.Timezone = loginData.Timezone;
                pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                Presences.BeginPresence(pr);

                Success(res, user.Id);
                res.Writer.Write(new ProtocolNegotiation());
                res.Writer.Write(new UserPresence(pr));
                res.Writer.Write(new PresenceBundle(Presences.GetUserIds(pr)));
                res.Writer.Write(new HandleUpdate(pr));
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
            }
        }

        public void LoginFailed(Res res) => res.Writer.Write(new LoginResponse(LoginResponses.Failed));
        public void Exception(Res res) => res.Writer.Write(new LoginResponse(LoginResponses.Exception));
        public void Success(Res res, int userid) => res.Writer.Write(new LoginResponse((LoginResponses)userid));
    }
}
