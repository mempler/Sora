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

using Sora.Database.Models;
using Sora.Enums;
using Sora.Extensions;
using Sora.Helpers;
using Sora.Interfaces;
using Sora.Objects;
using Sora.Packets.Client;

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
            if (Presence["STATUS"] == null)
                Presence["STATUS"] = new UserStatus
                {
                    Status = Status.Unknown
                };

            var userStatus = Presence.Get<UserStatus>("STATUS");
            
            sw.Write(Presence.User.Id);
            sw.Write((byte) userStatus.Status);
            sw.Write(userStatus.StatusText, false);
            sw.Write(userStatus.BeatmapChecksum, false);
            sw.Write((uint) userStatus.CurrentMods);
            sw.Write((byte) userStatus.Playmode);
            sw.Write(userStatus.BeatmapId);

            if (Presence["IS_RELAXING"] == null)
                Presence["IS_RELAXING"] = false;

            if (Presence["LB_RANK"] == null)
                Presence["LB_RANK"] = 0;
                
            switch (userStatus.Playmode)
            {
                case PlayMode.Osu:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").RankedScoreOsu
                            : Presence.Get<LeaderboardStd>("LB_STD").RankedScoreOsu
                    );
                    sw.Write(
                        (float) Presence.Get<double>("ACCURACY")
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PlayCountOsu
                            : Presence.Get<LeaderboardStd>("LB_STD").PlayCountOsu)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").TotalScoreOsu
                            : Presence.Get<LeaderboardStd>("LB_STD").TotalScoreOsu
                    );
                    sw.Write(Presence["LB_RANK"].As<uint>());
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PerformancePointsOsu
                            : Presence.Get<LeaderboardStd>("LB_STD").PerformancePointsOsu)
                    );
                    break;
                case PlayMode.Taiko:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").RankedScoreTaiko
                            : Presence.Get<LeaderboardStd>("LB_STD").RankedScoreTaiko
                    );
                    sw.Write(
                        (float) Presence.Get<double>("ACCURACY")
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PlayCountTaiko
                            : Presence.Get<LeaderboardStd>("LB_STD").PlayCountTaiko)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").TotalScoreTaiko
                            : Presence.Get<LeaderboardStd>("LB_STD").TotalScoreTaiko
                    );
                    sw.Write(Presence["LB_RANK"].As<uint>());
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PerformancePointsTaiko
                            : Presence.Get<LeaderboardStd>("LB_STD").PerformancePointsTaiko)
                    );
                    break;
                case PlayMode.Ctb:
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").RankedScoreCtb
                            : Presence.Get<LeaderboardStd>("LB_STD").RankedScoreCtb
                    );
                    sw.Write(
                        (float) Presence.Get<double>("ACCURACY")
                    );
                    sw.Write(
                        (uint) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PlayCountCtb
                            : Presence.Get<LeaderboardStd>("LB_STD").PlayCountCtb)
                    );
                    sw.Write(
                        Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").TotalScoreCtb
                            : Presence.Get<LeaderboardStd>("LB_STD").TotalScoreCtb
                    );
                    sw.Write((uint) Presence["LB_RANK"]);
                    sw.Write(
                        (ushort) (Presence.Get<bool>("IS_RELAXING")
                            ? Presence.Get<LeaderboardRx>("LB_RX").PerformancePointsCtb
                            : Presence.Get<LeaderboardStd>("LB_STD").PerformancePointsCtb)
                    );
                    break;
                case PlayMode.Mania:
                    sw.Write(Presence.Get<LeaderboardStd>("LB_STD").RankedScoreMania);
                    sw.Write(
                        (float) Presence.Get<double>("ACCURACY")
                    );
                    sw.Write((uint) Presence.Get<LeaderboardStd>("LB_STD").PlayCountMania);
                    sw.Write(Presence.Get<LeaderboardStd>("LB_STD").TotalScoreMania);
                    sw.Write(Presence["LB_RANK"].As<uint>());
                    sw.Write((ushort) Presence.Get<LeaderboardStd>("LB_STD").PerformancePointsMania);
                    break;
                default:
                    sw.Write((ulong) 0);
                    sw.Write((float) 0);
                    sw.Write((uint) 0);
                    sw.Write((ulong) 0);
                    sw.Write((ulong) 0);
                    sw.Write((ushort) 0);
                    break;
            }
        }
    }
}
