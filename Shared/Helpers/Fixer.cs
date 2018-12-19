using Shared.Enums;

namespace Jibril.Helpers
{
    public static class Fixer
    {
        public static RankedStatus FixRankedStatus(opi.v1.RankedStatus rankedStatus)
        {
            switch (rankedStatus)
            {
                case opi.v1.RankedStatus.Graveyard:
                    return RankedStatus.LatestPending;
                case opi.v1.RankedStatus.Wip:
                    return RankedStatus.LatestPending;
                case opi.v1.RankedStatus.Pending:
                    return RankedStatus.LatestPending;
                case opi.v1.RankedStatus.Ranked:
                    return RankedStatus.Ranked;
                case opi.v1.RankedStatus.Approved:
                    return RankedStatus.Approved;
                case opi.v1.RankedStatus.Qualified:
                    return RankedStatus.Qualified;
                case opi.v1.RankedStatus.Loved:
                    return RankedStatus.Loved;
                default:
                    return RankedStatus.Unknown;
            }
        }
    }
}