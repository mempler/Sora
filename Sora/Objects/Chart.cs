namespace Sora.Objects
{
    public class Chart
    {
        private readonly double _accuracyAfter;
        private readonly double _accuracyBefore;
        private readonly string _achievementsNew;
        private readonly string _chartId;
        private readonly string _chartName;
        private readonly string _chartUrl;
        private readonly int _maxComboAfter;
        private readonly int _maxComboBefore;
        private readonly int _onlineScoreId;
        private readonly double _ppAfter;
        private readonly double _ppBefore;
        private readonly int _rankAfter;
        private readonly int _rankBefore;
        private readonly int _rankedScoreBefore;
        private readonly int _totalScoreAfter;

        public Chart(string chartId,
            string chartName,
            string chartUrl,
            int rankBefore,
            int rankAfter,
            int maxComboBefore,
            int maxComboAfter,
            double accuracyBefore,
            double accuracyAfter,
            ulong rankedScoreBefore,
            ulong totalScoreAfter,
            double ppBefore,
            double ppAfter,
            int onlineScoreId,
            string achievements_new = null
        )
        {
            _chartId = chartId;
            _chartName = chartName;
            _chartUrl = chartUrl;
            _rankBefore = rankBefore;
            _rankAfter = rankAfter;
            _maxComboBefore = maxComboBefore;
            _maxComboAfter = maxComboAfter;
            _accuracyBefore = accuracyBefore;
            _accuracyAfter = accuracyAfter;
            _rankedScoreBefore = (int) rankedScoreBefore;
            _totalScoreAfter = (int) totalScoreAfter;
            _ppBefore = ppBefore;
            _ppAfter = ppAfter;
            _onlineScoreId = onlineScoreId;
            _achievementsNew = achievements_new;
        }

        public string ToOsuString()
            => $"chartId:{_chartId}" +
               $"|chartUrl:{_chartUrl}" +
               $"|chartName:{_chartName}" +
               $"|rankBefore:{_rankBefore}" +
               $"|rankAfter:{_rankAfter}" +
               $"|maxComboBefore:{_maxComboBefore}" +
               $"|maxComboAfter:{_maxComboAfter}" +
               $"|accuracyBefore:{_accuracyBefore}" +
               $"|accuracyAfter:{_accuracyAfter}" +
               $"|rankedScoreBefore:{_rankedScoreBefore}" +
               $"|rankedScoreAfter:{_rankedScoreBefore}" +
               $"|totalScoreBefore:{_totalScoreAfter}" +
               $"|totalScoreAfter:{_totalScoreAfter}" +
               $"|ppBefore:{_ppBefore}" +
               $"|ppAfter:{_ppAfter}" +
               (_achievementsNew == null ? "" : "|achievements-new:" + _achievementsNew) +
               $"|onlineScoreId:{_onlineScoreId}";
    }
}
