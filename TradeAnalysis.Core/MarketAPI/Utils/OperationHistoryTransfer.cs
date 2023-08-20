using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

public class OperationHistoryTransfer : OperationHistoryBase
{
    [JsonPropertyName("m_from")]
    public string? FromIdString { get; set; }
    public long? FromId
        => GetLong(FromIdString);

    [JsonPropertyName("m_to")]
    public string? ToIdString { get; set; }
    public long? ToId
        => GetLong(ToIdString);

    [JsonPropertyName("m_amount")]
    public string? AmountFromString { get; set; }
    public long? AmountFrom
        => GetLong(AmountFromString);

    [JsonPropertyName("m_currency")]
    public string? CurrencyFromString { get; set; }
    public Currency? CurrencyFrom
        => GetCurrency(CurrencyFromString);
    public double? RateFrom
        => GetRate(CurrencyFrom);

    [JsonPropertyName("m_amount_to")]
    public string? AmountToString { get; set; }
    public long? AmountTo
        => GetLong(AmountToString);

    [JsonPropertyName("m_currency_to")]
    public string? CurrencyToString { get; set; }
    public Currency? CurrencyTo
        => GetCurrency(CurrencyToString);
    public double? RateTo
        => GetRate(CurrencyTo);
}
