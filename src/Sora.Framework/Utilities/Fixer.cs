using Sora.Framework.Enums;

namespace Sora.Framework.Utilities
{
    public static class Fixer
    {
        public static RankedStatus FixRankedStatus(Pisstaube.RankedStatus rankedStatus)
        {
            switch (rankedStatus)
            {
                case Pisstaube.RankedStatus.Graveyard:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Wip:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Pending:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Ranked:
                    return RankedStatus.Ranked;
                case Pisstaube.RankedStatus.Approved:
                    return RankedStatus.Approved;
                case Pisstaube.RankedStatus.Qualified:
                    return RankedStatus.Qualified;
                case Pisstaube.RankedStatus.Loved:
                    return RankedStatus.Loved;
                default:
                    return RankedStatus.Unknown;
            }
        }
    }
}
