using TradeAnalysis.Core.Utils.MarketItems;

namespace TradeAnalysis.Core.Utils
{
    public class ProfitInfo
    {
        private readonly double _value;
        private readonly double _percent;
        private readonly double _duration;
        private readonly double _hourly;

        public ProfitInfo(DealInfo buyInfo, DealInfo sellInfo)
        {
            _value = sellInfo.Amount + buyInfo.Amount;
            _percent = _value / buyInfo.Amount;
            _duration = (sellInfo.Time - buyInfo.Time).TotalHours;
            _hourly = _value / _duration;
        }

        public double Value
        {
            get => _value;
        }

        public double Percent
        {
            get => _percent;
        }

        public double Duration
        {
            get => _duration;
        }

        public double Hourly
        {
            get => _hourly;
        }
    }
}
