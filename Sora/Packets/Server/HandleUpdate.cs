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

using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class HandleUpdate : IPacket
    {
        public Presence Presence;

        public HandleUpdate(Presence presence)
        {
            Presence = presence;
        }

        public PacketId Id => PacketId.ServerHandleOsuUpdate;

        public void ReadFromStream(MStreamReader sr)
        {
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Presence.User.Id);
            sw.Write((byte) Presence.Status.Status);
            sw.Write(Presence.Status.StatusText, false);
            sw.Write(Presence.Status.BeatmapChecksum, false);
            sw.Write(Presence.Status.CurrentMods);
            sw.Write((byte) Presence.Status.Playmode);
            sw.Write(Presence.Status.BeatmapId);
            if (Presence.Relax)
                switch (Presence.Status.Playmode)
                {
                    case PlayMode.Osu:
                        sw.Write(Presence.LeaderboardRx.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardRx.Count300Osu,
                                                              Presence.LeaderboardRx.Count100Osu,
                                                              Presence.LeaderboardRx.Count50Osu,
                                                              Presence.LeaderboardRx.CountMissOsu, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardRx.PlayCountOsu);
                        sw.Write(Presence.LeaderboardRx.TotalScoreOsu);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardRx.PerformancePointsOsu);
                        break;
                    case PlayMode.Taiko:
                        sw.Write(Presence.LeaderboardRx.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardRx.Count300Taiko,
                                                              Presence.LeaderboardRx.Count100Taiko,
                                                              Presence.LeaderboardRx.Count50Taiko,
                                                              Presence.LeaderboardRx.CountMissTaiko, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardRx.PlayCountTaiko);
                        sw.Write(Presence.LeaderboardRx.TotalScoreTaiko);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardRx.PerformancePointsTaiko);
                        break;
                    case PlayMode.Ctb:
                        sw.Write(Presence.LeaderboardRx.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardRx.Count300Ctb,
                                                              Presence.LeaderboardRx.Count100Ctb,
                                                              Presence.LeaderboardRx.Count50Ctb,
                                                              Presence.LeaderboardRx.CountMissCtb, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardRx.PlayCountCtb);
                        sw.Write(Presence.LeaderboardRx.TotalScoreCtb);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardRx.PerformancePointsCtb);
                        break;
                    case PlayMode.Mania:
                        sw.Write(Presence.LeaderboardStd.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardStd.Count300Mania,
                                                              Presence.LeaderboardStd.Count100Mania,
                                                              Presence.LeaderboardStd.Count50Mania,
                                                              Presence.LeaderboardStd.CountMissMania, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardStd.PlayCountMania);
                        sw.Write(Presence.LeaderboardStd.TotalScoreMania);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardStd.PerformancePointsMania);
                        break;
                    default:
                        sw.Write((ulong) 0);
                        sw.Write((float) 0);
                        sw.Write((uint) 0);
                        sw.Write((ulong) 0);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) 0);
                        break;
                }
            else
                switch (Presence.Status.Playmode)
                {
                    case PlayMode.Osu:
                        sw.Write(Presence.LeaderboardStd.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardStd.Count300Osu,
                                                              Presence.LeaderboardStd.Count100Osu,
                                                              Presence.LeaderboardStd.Count50Osu,
                                                              Presence.LeaderboardStd.CountMissOsu, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardStd.PlayCountOsu);
                        sw.Write(Presence.LeaderboardStd.TotalScoreOsu);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardStd.PerformancePointsOsu);
                        break;
                    case PlayMode.Taiko:
                        sw.Write(Presence.LeaderboardStd.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardStd.Count300Taiko,
                                                              Presence.LeaderboardStd.Count100Taiko,
                                                              Presence.LeaderboardStd.Count50Taiko,
                                                              Presence.LeaderboardStd.CountMissTaiko, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardStd.PlayCountTaiko);
                        sw.Write(Presence.LeaderboardStd.TotalScoreTaiko);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardStd.PerformancePointsTaiko);
                        break;
                    case PlayMode.Ctb:
                        sw.Write(Presence.LeaderboardStd.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardStd.Count300Ctb,
                                                              Presence.LeaderboardStd.Count100Ctb,
                                                              Presence.LeaderboardStd.Count50Ctb,
                                                              Presence.LeaderboardStd.CountMissCtb, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardStd.PlayCountCtb);
                        sw.Write(Presence.LeaderboardStd.TotalScoreCtb);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardStd.PerformancePointsCtb);
                        break;
                    case PlayMode.Mania:
                        sw.Write(Presence.LeaderboardStd.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardStd.Count300Mania,
                                                              Presence.LeaderboardStd.Count100Mania,
                                                              Presence.LeaderboardStd.Count50Mania,
                                                              Presence.LeaderboardStd.CountMissMania, 0, 0,
                                                              Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardStd.PlayCountMania);
                        sw.Write(Presence.LeaderboardStd.TotalScoreMania);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardStd.PerformancePointsMania);
                        break;
                    default:
                        sw.Write((ulong) 0);
                        sw.Write((float) 0);
                        sw.Write((uint) 0);
                        sw.Write((ulong) 0);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) 0);
                        break;
                }
        }
    }
}