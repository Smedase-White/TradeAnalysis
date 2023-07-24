namespace TradeOnAnalysis.Core.Utils
{
    public class ProfitInfo
    {
        public double Value { get; init; }
        public double Percent { get; init; }
        public int Duration { get; init; }
        public double Daily { get; init; }

        public ProfitInfo(ActionInfo buyInfo, ActionInfo sellInfo)
        {
            Value = sellInfo.Price - buyInfo.Price;
            Percent = Value / buyInfo.Price;
            Duration = (sellInfo.Date - buyInfo.Date).Days;
            Daily = Value / Duration;
        }
    }
}
