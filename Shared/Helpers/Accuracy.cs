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
            if (count300 + count100 + count100 + count50 + countMiss + countGeki + countKatu == 0)
                return 1;
            switch (playMode)
            {
                case PlayModes.Osu:
                    return count50 * 50 + count100 * 100 + count300 * 300 / ((double) countMiss + count50 + count300 + count100 * 300);
                case PlayModes.Taiko:
                    return  count50 * 50 + count300 * 100 / ((double) countMiss + count100 + count300 * 100);
                case PlayModes.Ctb:
                    return count300 + count100 + count50 / (double) count300 + count100 + count50 + count300 + countKatu;
                case PlayModes.Mania:
                    return count50 * 50 + count100 * 100 + countKatu * 200 + count300 * 300 + countGeki * 300 / ((double) countMiss + count50 + count100 + count300 + countGeki + countKatu * 300);
                default:
                    return 0;
            }
        }
    }
}
