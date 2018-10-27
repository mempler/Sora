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

namespace Sora.Packets.Server
{
    using Objects;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class HandleUpdate : IPacket
    {
        public Presence Presence;

        public HandleUpdate(Presence presence) => Presence = presence;

        public PacketId Id => PacketId.ServerHandleOsuUpdate;

        public void ReadFromStream(MStreamReader sr) { }

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
                        sw.Write((ushort) Presence.LeaderboardRx.PeppyPointsOsu);
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
                        sw.Write((ushort) Presence.LeaderboardRx.PeppyPointsTaiko);
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
                        sw.Write((ushort) Presence.LeaderboardRx.PeppyPointsCtb);
                        break;
                    case PlayMode.Mania:
                        sw.Write(Presence.LeaderboardRx.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardRx.Count300Mania,
                            Presence.LeaderboardRx.Count100Mania,
                            Presence.LeaderboardRx.Count50Mania,
                            Presence.LeaderboardRx.CountMissMania, 0, 0,
                            Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardRx.PlayCountMania);
                        sw.Write(Presence.LeaderboardRx.TotalScoreMania);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardRx.PeppyPointsMania);
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
            else if (Presence.Touch)
                switch (Presence.Status.Playmode)
                {
                    case PlayMode.Osu:
                        sw.Write(Presence.LeaderboardTouch.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardTouch.Count300Osu,
                            Presence.LeaderboardTouch.Count100Osu,
                            Presence.LeaderboardTouch.Count50Osu,
                            Presence.LeaderboardTouch.CountMissOsu, 0, 0,
                            Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardTouch.PlayCountOsu);
                        sw.Write(Presence.LeaderboardTouch.TotalScoreOsu);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardTouch.PeppyPointsOsu);
                        break;
                    case PlayMode.Taiko:
                        sw.Write(Presence.LeaderboardTouch.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardTouch.Count300Taiko,
                            Presence.LeaderboardTouch.Count100Taiko,
                            Presence.LeaderboardTouch.Count50Taiko,
                            Presence.LeaderboardTouch.CountMissTaiko, 0, 0,
                            Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardTouch.PlayCountTaiko);
                        sw.Write(Presence.LeaderboardTouch.TotalScoreTaiko);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardTouch.PeppyPointsTaiko);
                        break;
                    case PlayMode.Ctb:
                        sw.Write(Presence.LeaderboardTouch.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardTouch.Count300Ctb,
                            Presence.LeaderboardTouch.Count100Ctb,
                            Presence.LeaderboardTouch.Count50Ctb,
                            Presence.LeaderboardTouch.CountMissCtb, 0, 0,
                            Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardTouch.PlayCountCtb);
                        sw.Write(Presence.LeaderboardTouch.TotalScoreCtb);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardTouch.PeppyPointsCtb);
                        break;
                    case PlayMode.Mania:
                        sw.Write(Presence.LeaderboardTouch.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(Presence.LeaderboardTouch.Count300Mania,
                            Presence.LeaderboardTouch.Count100Mania,
                            Presence.LeaderboardTouch.Count50Mania,
                            Presence.LeaderboardTouch.CountMissMania, 0, 0,
                            Presence.Status.Playmode));
                        sw.Write((uint) Presence.LeaderboardTouch.PlayCountMania);
                        sw.Write(Presence.LeaderboardTouch.TotalScoreMania);
                        sw.Write(Presence.Rank);
                        sw.Write((ushort) Presence.LeaderboardTouch.PeppyPointsMania);
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
                        sw.Write((ushort) Presence.LeaderboardStd.PeppyPointsOsu);
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
                        sw.Write((ushort) Presence.LeaderboardStd.PeppyPointsTaiko);
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
                        sw.Write((ushort) Presence.LeaderboardStd.PeppyPointsCtb);
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
                        sw.Write((ushort) Presence.LeaderboardStd.PeppyPointsMania);
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
