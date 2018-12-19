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

using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using SpectatorFrames = Sora.Packets.Server.SpectatorFrames;

namespace Sora.Handler
{
    public class SpectatorHandler
    {
        [Handler(HandlerTypes.BanchoStartSpectating)]
        public void OnStartSpectating(Presence pr, int userId)
        {
            Presence opr = LPresences.GetPresence(userId);
            if (opr == null) return;
            if (opr.Spectator == null)
            {
                opr.Spectator = new SpectatorStream($"spec-{userId}", opr);
                opr.Spectator.Join(opr);
                opr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));
            }

            pr.Spectator = opr.Spectator;

            opr.Spectator.Join(pr);
            if (opr.Spectator.SpecChannel.JoinChannel(pr))
                pr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));

            opr.Spectator.Broadcast(new FellowSpectatorJoined(pr.User.Id));
            opr.Write(new SpectatorJoined(pr.User.Id));
        }

        [Handler(HandlerTypes.BanchoStopSpectating)]
        public void OnStopSpectating(Presence pr)
        {
            if (pr?.Spectator == null) return;
            Presence opr = pr.Spectator.BoundPresence;

            opr.Write(new FellowSpectatorLeft(pr.User.Id));
            opr.Spectator.Broadcast(new SpectatorLeft(pr.User.Id));

            opr.Spectator.Left(pr);
            opr.Spectator.SpecChannel.LeaveChannel(pr);
            pr.Write(new ChannelRevoked(opr.Spectator.SpecChannel));

            pr.Spectator = null;

            if (opr.Spectator.JoinedUsers > 0) return;

            opr.Spectator.Left(opr);
            opr.Spectator.SpecChannel.LeaveChannel(opr);
            opr.Write(new ChannelRevoked(opr.Spectator.SpecChannel));
            opr.Spectator = null;
        }

        [Handler(HandlerTypes.BanchoCantSpectate)]
        public void OnUserCantSpectate(Presence pr)
        {
            pr.Spectator?.Broadcast(new SpectatorCantSpectate(pr.User.Id));
        }

        [Handler(HandlerTypes.BanchoSpectateFrames)]
        public void OnBroadcastingFrames(Presence pr, SpectatorFrame frames)
        {
            pr.Spectator?.Broadcast(new SpectatorFrames(frames), pr);
        }
    }
}