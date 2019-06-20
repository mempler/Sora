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

using Sora.Enums;
using Sora.Helpers;
using Sora.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class HandleUpdate : IPacket
    {
        public Presence Presence;

        public HandleUpdate(Presence presence) => Presence = presence;

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
            sw.Write((uint) Presence.Status.CurrentMods);
            sw.Write((byte) Presence.Status.Playmode);
            sw.Write(Presence.Status.BeatmapId);

            switch (Presence.Status.Playmode)
            {
                case PlayMode.Osu:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.RankedScoreOsu
                            : Presence.LeaderboardStd.RankedScoreOsu
                    );
                    sw.Write(
                        (float) Accuracy.GetAccuracy(
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count300Osu
                                : Presence.LeaderboardStd.Count300Osu,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count100Osu
                                : Presence.LeaderboardStd.Count100Osu,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count50Osu
                                : Presence.LeaderboardStd.Count50Osu,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.CountMissOsu
                                : Presence.LeaderboardStd.CountMissOsu, 0, 0,
                            Presence.Status.Playmode
                        )
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PlayCountOsu
                            : Presence.LeaderboardStd.PlayCountOsu)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.TotalScoreOsu
                            : Presence.LeaderboardStd.TotalScoreOsu
                    );
                    sw.Write(Presence.Rank);
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PerformancePointsOsu
                            : Presence.LeaderboardStd.PerformancePointsOsu)
                    );
                    break;
                case PlayMode.Taiko:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.RankedScoreTaiko
                            : Presence.LeaderboardStd.RankedScoreTaiko
                    );
                    sw.Write(
                        (float) Accuracy.GetAccuracy(
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count300Taiko
                                : Presence.LeaderboardStd.Count300Taiko,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count100Taiko
                                : Presence.LeaderboardStd.Count100Taiko,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count50Taiko
                                : Presence.LeaderboardStd.Count50Taiko,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.CountMissTaiko
                                : Presence.LeaderboardStd.CountMissTaiko, 0, 0,
                            Presence.Status.Playmode
                        )
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PlayCountTaiko
                            : Presence.LeaderboardStd.PlayCountTaiko)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.TotalScoreTaiko
                            : Presence.LeaderboardStd.TotalScoreTaiko
                    );
                    sw.Write(Presence.Rank);
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PerformancePointsTaiko
                            : Presence.LeaderboardStd.PerformancePointsTaiko)
                    );
                    break;
                case PlayMode.Ctb:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.RankedScoreCtb
                            : Presence.LeaderboardStd.RankedScoreCtb
                    );
                    sw.Write(
                        (float) Accuracy.GetAccuracy(
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count300Ctb
                                : Presence.LeaderboardStd.Count300Ctb,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count100Ctb
                                : Presence.LeaderboardStd.Count100Ctb,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.Count50Ctb
                                : Presence.LeaderboardStd.Count50Ctb,
                            Presence.Get<bool>("IS_RELAXING")
                                ? Presence.LeaderboardRx.CountMissCtb
                                : Presence.LeaderboardStd.CountMissCtb, 0, 0,
                            Presence.Status.Playmode
                        )
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PlayCountCtb
                            : Presence.LeaderboardStd.PlayCountCtb)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.TotalScoreCtb
                            : Presence.LeaderboardStd.TotalScoreCtb
                    );
                    sw.Write(Presence.Rank);
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.LeaderboardRx.PerformancePointsCtb
                            : Presence.LeaderboardStd.PerformancePointsCtb)
                    );
                    break;
                case PlayMode.Mania:
                    sw.Write(Presence.LeaderboardStd.RankedScoreMania);
                    sw.Write(
                        (float) Accuracy.GetAccuracy(
                            Presence.LeaderboardStd.Count300Mania,
                            Presence.LeaderboardStd.Count100Mania,
                            Presence.LeaderboardStd.Count50Mania,
                            Presence.LeaderboardStd.CountMissMania, 0, 0,
                            Presence.Status.Playmode
                        )
                    );
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
