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
        private readonly SoraDbContext _ctx;

        public OnSendUserStatusEvent(PresenceService ps, SoraDbContext ctx)
        {
            _ps = ps;
            _ctx = ctx;
        }


        [Event(EventType.BanchoSendUserStatus)]
        public void OnSendUserStatus(BanchoSendUserStatusArgs args)
        {
            if (args.Pr.Status.Playmode != args.Status.Playmode)
            {
                var lb = (DbLeaderboard) args.Pr["LB"];
                args.Pr.Stats.Accuracy = (float) lb.GetAccuracy(_ctx, args.Status.Playmode);
                args.Pr.Stats.Position = lb.GetPosition(_ctx, args.Status.Playmode);
                switch (args.Status.Playmode)
                {
                    case PlayMode.Osu:
                        args.Pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsOsu;
                        args.Pr.Stats.TotalScore = (ushort) lb.TotalScoreOsu;
                        args.Pr.Stats.RankedScore = (ushort) lb.RankedScoreOsu;
                        args.Pr.Stats.PlayCount = (ushort) lb.PlayCountOsu;
                        break;
                    case PlayMode.Taiko:
                        args.Pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsTaiko;
                        args.Pr.Stats.TotalScore = (ushort) lb.TotalScoreTaiko;
                        args.Pr.Stats.RankedScore = (ushort) lb.RankedScoreTaiko;
                        args.Pr.Stats.PlayCount = (ushort) lb.PlayCountTaiko;
                        break;
                    case PlayMode.Ctb:
                        args.Pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsCtb;
                        args.Pr.Stats.TotalScore = (ushort) lb.TotalScoreCtb;
                        args.Pr.Stats.RankedScore = (ushort) lb.RankedScoreCtb;
                        args.Pr.Stats.PlayCount = (ushort) lb.PlayCountCtb;
                        break;
                    case PlayMode.Mania:
                        args.Pr.Stats.PerformancePoints = (ushort) lb.PerformancePointsMania;
                        args.Pr.Stats.TotalScore = (ushort) lb.TotalScoreMania;
                        args.Pr.Stats.RankedScore = (ushort) lb.RankedScoreMania;
                        args.Pr.Stats.PlayCount = (ushort) lb.PlayCountMania;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            args.Pr.Status = args.Status;
            _ps.Push(new HandleUpdate(args.Pr));
        }
    }
}
