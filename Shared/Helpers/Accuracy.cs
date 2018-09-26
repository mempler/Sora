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

namespace Shared.Helpers
{
    public static class Accuracy
    {
        public static double GetAccuracy(ulong count300, ulong count100, ulong count50, ulong countMiss,
            ulong countGeki, ulong countKatu, PlayModes playMode)
        {
            ulong totalHitPoints;
            ulong totalHits;
            switch (playMode)
            {
                case PlayModes.Osu:
                    totalHitPoints = count50 * 50 + count100 * 100 + count300 * 300;
                    totalHits = countMiss + count50 + count300 + count100;
                    return totalHitPoints / ((double) totalHits * 300);
                case PlayModes.Taiko:
                    totalHitPoints = count50 * 50 + count300 * 100;
                    totalHits = countMiss + count100 + count300;
                    return totalHitPoints / ((double) totalHits * 100);
                case PlayModes.Ctb:
                    totalHitPoints = count300 + count100 + count50;
                    totalHits = totalHitPoints + count300 + countKatu;
                    return totalHitPoints / (double)totalHits;
                case PlayModes.Mania:
                    totalHitPoints = count50 * 50 + count100 * 100 + countKatu * 200 + count300 * 300 + countGeki * 300;
                    totalHits = countMiss + count50 + count100 + count300 + countGeki + countKatu;
                    return totalHitPoints / ((double)totalHits * 300);
                default:
                    return 0;
            }
        }
    }
}
