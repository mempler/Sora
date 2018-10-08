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
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class HandleUpdate : IPacket
    {
        public PacketId Id => PacketId.ServerHandleOsuUpdate;
        public Presence Presence;

        public HandleUpdate(Presence presence) => this.Presence = presence;

        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();
        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(this.Presence.User.Id);
            sw.Write((byte) this.Presence.Status.Status);
            sw.Write(this.Presence.Status.StatusText, false);
            sw.Write(this.Presence.Status.BeatmapChecksum, false);
            sw.Write(this.Presence.Status.CurrentMods);
            sw.Write((byte) this.Presence.Status.Playmode);
            sw.Write(this.Presence.Status.BeatmapId);
            if (this.Presence.Relax)
            {
                switch (this.Presence.Status.Playmode)
                {
                    case PlayModes.Osu:
                        sw.Write(this.Presence.LeaderboardRx.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardRx.Count300Osu, this.Presence.LeaderboardRx.Count100Osu, this.Presence.LeaderboardRx.Count50Osu, this.Presence.LeaderboardRx.CountMissOsu, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardRx.PlayCountOsu);
                        sw.Write(this.Presence.LeaderboardRx.TotalScoreOsu);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardRx.PeppyPointsOsu);
                        break;
                    case PlayModes.Taiko:
                        sw.Write(this.Presence.LeaderboardRx.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardRx.Count300Taiko, this.Presence.LeaderboardRx.Count100Taiko, this.Presence.LeaderboardRx.Count50Taiko, this.Presence.LeaderboardRx.CountMissTaiko, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardRx.PlayCountTaiko);
                        sw.Write(this.Presence.LeaderboardRx.TotalScoreTaiko);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardRx.PeppyPointsTaiko);
                        break;
                    case PlayModes.Ctb:
                        sw.Write(this.Presence.LeaderboardRx.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardRx.Count300Ctb, this.Presence.LeaderboardRx.Count100Ctb, this.Presence.LeaderboardRx.Count50Ctb, this.Presence.LeaderboardRx.CountMissCtb, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardRx.PlayCountCtb);
                        sw.Write(this.Presence.LeaderboardRx.TotalScoreCtb);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardRx.PeppyPointsCtb);
                        break;
                    case PlayModes.Mania:
                        sw.Write(this.Presence.LeaderboardRx.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardRx.Count300Mania, this.Presence.LeaderboardRx.Count100Mania, this.Presence.LeaderboardRx.Count50Mania, this.Presence.LeaderboardRx.CountMissMania, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardRx.PlayCountMania);
                        sw.Write(this.Presence.LeaderboardRx.TotalScoreMania);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardRx.PeppyPointsMania);
                        break;
                    default:
                        sw.Write((ulong)0);
                        sw.Write((float)0);
                        sw.Write((uint)0);
                        sw.Write((ulong)0);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort)0);
                        break;
                }
            }
            else if (this.Presence.Touch)
            {
                switch (this.Presence.Status.Playmode)
                {
                    case PlayModes.Osu:
                        sw.Write(this.Presence.LeaderboardTouch.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardTouch.Count300Osu, this.Presence.LeaderboardTouch.Count100Osu, this.Presence.LeaderboardTouch.Count50Osu, this.Presence.LeaderboardTouch.CountMissOsu, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardTouch.PlayCountOsu);
                        sw.Write(this.Presence.LeaderboardTouch.TotalScoreOsu);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardTouch.PeppyPointsOsu);
                        break;
                    case PlayModes.Taiko:
                        sw.Write(this.Presence.LeaderboardTouch.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardTouch.Count300Taiko, this.Presence.LeaderboardTouch.Count100Taiko, this.Presence.LeaderboardTouch.Count50Taiko, this.Presence.LeaderboardTouch.CountMissTaiko, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardTouch.PlayCountTaiko);
                        sw.Write(this.Presence.LeaderboardTouch.TotalScoreTaiko);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardTouch.PeppyPointsTaiko);
                        break;
                    case PlayModes.Ctb:
                        sw.Write(this.Presence.LeaderboardTouch.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardTouch.Count300Ctb, this.Presence.LeaderboardTouch.Count100Ctb, this.Presence.LeaderboardTouch.Count50Ctb, this.Presence.LeaderboardTouch.CountMissCtb, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardTouch.PlayCountCtb);
                        sw.Write(this.Presence.LeaderboardTouch.TotalScoreCtb);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardTouch.PeppyPointsCtb);
                        break;
                    case PlayModes.Mania:
                        sw.Write(this.Presence.LeaderboardTouch.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardTouch.Count300Mania, this.Presence.LeaderboardTouch.Count100Mania, this.Presence.LeaderboardTouch.Count50Mania, this.Presence.LeaderboardTouch.CountMissMania, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardTouch.PlayCountMania);
                        sw.Write(this.Presence.LeaderboardTouch.TotalScoreMania);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardTouch.PeppyPointsMania);
                        break;
                    default:
                        sw.Write((ulong)0);
                        sw.Write((float)0);
                        sw.Write((uint)0);
                        sw.Write((ulong)0);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort)0);
                        break;
                }
            }
            else
            {
                switch (this.Presence.Status.Playmode)
                {
                    case PlayModes.Osu:
                        sw.Write(this.Presence.LeaderboardStd.RankedScoreOsu);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardStd.Count300Osu, this.Presence.LeaderboardStd.Count100Osu, this.Presence.LeaderboardStd.Count50Osu, this.Presence.LeaderboardStd.CountMissOsu, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardStd.PlayCountOsu);
                        sw.Write(this.Presence.LeaderboardStd.TotalScoreOsu);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardStd.PeppyPointsOsu);
                        break;
                    case PlayModes.Taiko:
                        sw.Write(this.Presence.LeaderboardStd.RankedScoreTaiko);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardStd.Count300Taiko, this.Presence.LeaderboardStd.Count100Taiko, this.Presence.LeaderboardStd.Count50Taiko, this.Presence.LeaderboardStd.CountMissTaiko, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardStd.PlayCountTaiko);
                        sw.Write(this.Presence.LeaderboardStd.TotalScoreTaiko);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardStd.PeppyPointsTaiko);
                        break;
                    case PlayModes.Ctb:
                        sw.Write(this.Presence.LeaderboardStd.RankedScoreCtb);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardStd.Count300Ctb, this.Presence.LeaderboardStd.Count100Ctb, this.Presence.LeaderboardStd.Count50Ctb, this.Presence.LeaderboardStd.CountMissCtb, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardStd.PlayCountCtb);
                        sw.Write(this.Presence.LeaderboardStd.TotalScoreCtb);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardStd.PeppyPointsCtb);
                        break;
                    case PlayModes.Mania:
                        sw.Write(this.Presence.LeaderboardStd.RankedScoreMania);
                        sw.Write((float) Accuracy.GetAccuracy(this.Presence.LeaderboardStd.Count300Mania, this.Presence.LeaderboardStd.Count100Mania, this.Presence.LeaderboardStd.Count50Mania, this.Presence.LeaderboardStd.CountMissMania, 0, 0, this.Presence.Status.Playmode));
                        sw.Write((uint) this.Presence.LeaderboardStd.PlayCountMania);
                        sw.Write(this.Presence.LeaderboardStd.TotalScoreMania);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort) this.Presence.LeaderboardStd.PeppyPointsMania);
                        break;
                    default:
                        sw.Write((ulong)0);
                        sw.Write((float)0);
                        sw.Write((uint)0);
                        sw.Write((ulong)0);
                        sw.Write(this.Presence.Rank);
                        sw.Write((ushort)0);
                        break;
                }
            }
        }
    }
}
