namespace TradeAnalysis.Core.MarketAPI;

public enum EventType
{
    Buy,
    Sell,
    PayIn,
    PayOut,
    TransferIn,
    TransferOut,
}

public enum Currency
{
    Rub,
    Usd,
    Eur,
}

public static class TypeUtils
{
    public static long? GetLong(string? longString)
        => longString is null ? null : Convert.ToInt64(longString);

    public static DateTime? GetTime(string? timeString)
        => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timeString)).LocalDateTime;

    public static EventType? GetEvent(string? eventString)
        => eventString switch
        {
            "buy_go" => EventType.Buy,
            "sell_go" => EventType.Sell,
            "checkin" => EventType.PayIn,
            "checkout" => EventType.PayOut,
            "moneyget" => EventType.TransferIn,
            "moneymove" => EventType.TransferOut,
            _ => null
        };

    public static Type? GetOperationTypeByEvent(EventType? eventType)
        => eventType switch
        {
            EventType.Buy => typeof(OperationHistoryItem),
            EventType.Sell => typeof(OperationHistoryItem),
            EventType.PayIn => typeof(OperationHistoryPay),
            EventType.PayOut => typeof(OperationHistoryPay),
            EventType.TransferIn => typeof(OperationHistoryTransfer),
            EventType.TransferOut => typeof(OperationHistoryTransfer),
            _ => null
        };

    public static Currency? GetCurrency(string? currencyString)
        => currencyString switch
        {
            "RUB" => Currency.Rub,
            "USD" => Currency.Usd,
            "EUR" => Currency.Eur,
            _ => null
        };

    public static double? GetRate(Currency? currency)
        => currency switch
        {
            Currency.Rub => 0.01,
            Currency.Usd => 1.0,
            Currency.Eur => 1.0,
            _ => null
        };
}
