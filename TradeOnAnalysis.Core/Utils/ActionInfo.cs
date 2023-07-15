namespace TradeOnAnalysis.Core.Utils;

public class ActionInfo
{
    public double Price { get; init; }
    public DateTime Date { get; init; }

    public ActionInfo(double price, DateTime date)
    {
        Price = price;
        Date = date;
    }
}