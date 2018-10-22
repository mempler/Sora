#region copyright

/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using MaxMind.GeoIP2.Responses;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Enums;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Handler
{
    internal class LoginHandler
    {
        [Handler(HandlerTypes.LoginHandler)]
        public void OnLogin(Presence pr, MStreamWriter dataWriter, MStreamReader dataReader, string Ip)
        {
            try
            {
                Login loginData = LoginParser.ParseLogin(dataReader);
                if (loginData == null)
                {
                    Exception(dataWriter);
                    return;
                }

                Users user = Users.GetUser(Users.GetUserId(loginData.Username));
                if (user == null)
                {
                    LoginFailed(dataWriter);
                    return;
                }

                if (!user.IsPassword(loginData.Password))
                {
                    LoginFailed(dataWriter);
                    return;
                }

                pr.User = user;

                if (Ip != "127.0.0.1" && Ip != "0.0.0.0")
                {
                    CityResponse data = Localisation.GetData(Ip);
                    pr.CountryId = Localisation.StringToCountryId(data.Country.IsoCode);
                    if (data.Location.Longitude != null) pr.Lon = (double) data.Location.Longitude;
                    if (data.Location.Latitude != null) pr.Lat  = (double) data.Location.Latitude;
                }

                pr.LeaderboardStd   = LeaderboardStd.GetLeaderboard(pr.User);
                pr.LeaderboardRx    = LeaderboardRx.GetLeaderboard(pr.User);
                pr.LeaderboardTouch = LeaderboardTouch.GetLeaderboard(pr.User);

                pr.Timezone         = loginData.Timezone;
                pr.BlockNonFriendDm = loginData.BlockNonFriendDMs;

                LPresences.BeginPresence(pr);

                Success(dataWriter, user.Id);
                dataWriter.Write(new ProtocolNegotiation());
                dataWriter.Write(new UserPresence(pr));
                dataWriter.Write(new PresenceBundle(LPresences.GetUserIds(pr).ToList()));
                dataWriter.Write(new HandleUpdate(pr));

                foreach (Channel chanAuto in Channels.ChannelsAutoJoin)
                {
                    if (chanAuto.AdminOnly && pr.User.HasPrivileges(Privileges.Admin))
                        dataWriter.Write(new ChannelAvailableAutojoin(chanAuto));
                    else if (!chanAuto.AdminOnly)
                        dataWriter.Write(new ChannelAvailableAutojoin(chanAuto));

                    if (chanAuto.JoinChannel(pr))
                        dataWriter.Write(new ChannelJoinSuccess(chanAuto));
                    else
                        dataWriter.Write(new ChannelRevoked(chanAuto));
                }

                foreach (KeyValuePair<string, Channel> chan in Channels.Channels_)
                    if (chan.Value.AdminOnly && pr.User.HasPrivileges(Privileges.Admin))
                        dataWriter.Write(new ChannelAvailable(chan.Value));
                    else if (!chan.Value.AdminOnly)
                        dataWriter.Write(new ChannelAvailable(chan.Value));

                PacketStream stream = LPacketStreams.GetStream("main");
                if (stream == null)
                {
                    Exception(dataWriter);
                    return;
                }

                stream.Broadcast(new PresenceSingle(pr.User.Id));
                stream.Broadcast(new HandleUpdate(pr));
                stream.Join(pr);
            }
            catch (Exception ex)
            {
                Logger.L.Error(ex);
                Exception(dataWriter);
            }
        }

        private static void LoginFailed(MStreamWriter dataWriter) => dataWriter.Write(new LoginResponse(LoginResponses.Failed));

        private static void Exception(MStreamWriter dataWriter) => dataWriter.Write(new LoginResponse(LoginResponses.Exception));

        private static void Success(MStreamWriter dataWriter, int userid) =>
            dataWriter.Write(new LoginResponse((LoginResponses) userid));
    }
}