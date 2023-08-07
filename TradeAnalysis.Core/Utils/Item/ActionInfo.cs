namespace TradeAnalysis.Core.Utils.Item;

public class ActionInfo
{
    public double Price { get; init; }
    public DateTime Time { get; init; }

    public ActionInfo(double price, DateTime time)
    {
        Price = price;
        Time = time;
    }
}