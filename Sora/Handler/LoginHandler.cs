using System;
using Shared.Database.Models;
using Shared.Enums;
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
                pr.Timezone = loginData.Timezone;
                pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                Presences.BeginPresence(pr);
                Console.WriteLine(user);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
            }
        }

        public void LoginFailed(Res res) => res.Writer.Write(new LoginResponse(LoginResponses.Failed));
        public void Exception(Res res) => res.Writer.Write(new LoginResponse(LoginResponses.Exception));
    }
}
