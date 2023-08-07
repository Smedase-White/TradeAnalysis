namespace TradeAnalysis.Core.Utils.Item;

public class ItemDailyPrices
{
    public List<double> Prices { get; } = new();

    public double AveragePrice
        => Prices.Sum() / Prices.Count;

    public double Count
        => Prices.Count;

    public ItemDailyPrices(params double[] prices)
    {
        foreach (double price in prices)
            Prices.Add(price);
    }

    public void AddPrice(double price) => Prices.Add(price);
}