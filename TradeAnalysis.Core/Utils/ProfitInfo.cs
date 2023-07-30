namespace TradeAnalysis.Core.Utils
{
    public class ProfitInfo
    {
        public double Value { get; init; }
        public double Percent { get; init; }
        public int Duration { get; init; }
        public double Hourly { get; init; }

        public ProfitInfo(ActionInfo buyInfo, ActionInfo sellInfo)
        {
            Value = sellInfo.Price - buyInfo.Price;
            Percent = Value / buyInfo.Price;
            Duration = (int)(sellInfo.Time - buyInfo.Time).TotalHours;
            Hourly = Value / Duration;
        }
    }
}
