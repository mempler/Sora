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
using Sora.Attributes;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;
using Sora.Framework.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnSendUserStatusEvent
    {
        private readonly PresenceService _ps;
        private readonly SoraDbContextFactory _factory;

        public OnSendUserStatusEvent(PresenceService ps, SoraDbContextFactory factory)
        {
            _ps = ps;
            _factory = factory;
        }


        [Event(EventType.BanchoSendUserStatus)]
        public void OnSendUserStatus(BanchoSendUserStatusArgs args)
        {
            if (args.pr.Status.Playmode != args.status.Playmode)
            {
                var lb = (DBLeaderboard) args.pr["LB"];
                args.pr.Stats.Accuracy = (float) lb.GetAccuracy(_factory, args.status.Playmode);
                args.pr.Stats.Position = lb.GetPosition(_factory, args.status.Playmode);
                switch (args.status.Playmode)
                {
                    case PlayMode.Osu:
                        args.pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsOsu;
                        args.pr.Stats.TotalScore = (ushort) lb.TotalScoreOsu;
                        args.pr.Stats.RankedScore = (ushort) lb.RankedScoreOsu;
                        args.pr.Stats.PlayCount = (ushort) lb.PlayCountOsu;
                        break;
                    case PlayMode.Taiko:
                        args.pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsTaiko;
                        args.pr.Stats.TotalScore = (ushort) lb.TotalScoreTaiko;
                        args.pr.Stats.RankedScore = (ushort) lb.RankedScoreTaiko;
                        args.pr.Stats.PlayCount = (ushort) lb.PlayCountTaiko;
                        break;
                    case PlayMode.Ctb:
                        args.pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsCtb;
                        args.pr.Stats.TotalScore = (ushort) lb.TotalScoreCtb;
                        args.pr.Stats.RankedScore = (ushort) lb.RankedScoreCtb;
                        args.pr.Stats.PlayCount = (ushort) lb.PlayCountCtb;
                        break;
                    case PlayMode.Mania:
                        args.pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsMania;
                        args.pr.Stats.TotalScore = (ushort) lb.TotalScoreMania;
                        args.pr.Stats.RankedScore = (ushort) lb.RankedScoreMania;
                        args.pr.Stats.PlayCount = (ushort) lb.PlayCountMania;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            args.pr.Status = args.status;
            _ps.Push(new HandleUpdate(args.pr));
        }
    }
}
