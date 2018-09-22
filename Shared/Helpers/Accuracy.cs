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
