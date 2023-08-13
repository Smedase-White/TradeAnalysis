using System.Text.Json.Serialization;

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

public class OperationHistoryBase
{
    [JsonPropertyName("h_id")]
    public string? IdString { get; set; }
    public long? Id
        => GetLong(IdString);

    [JsonPropertyName("h_event_id")]
    public string? EventIdString { get; set; }
    public long? EventId
        => GetLong(EventIdString);

    [JsonPropertyName("h_event")]
    public string? EventString { get; set; }
    public EventType? Event
        => GetEvent(EventString);

    [JsonPropertyName("h_time")]
    public string? TimeString { get; set; }
    public DateTime Time
        => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(TimeString)).LocalDateTime;

    public static long? GetLong(string? longString)
        => longString is null ? null : Convert.ToInt64(longString);

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
