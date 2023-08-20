using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

public class OperationHistoryPay : OperationHistoryBase
{
    [JsonPropertyName("join")]
    public int? Join { get; set; }

    [JsonPropertyName("app")]
    public string? App { get; set; }

    [JsonPropertyName("id")]
    public string? PayIdString { get; set; }
    public long? PayId
        => GetLong(PayIdString);

    [JsonPropertyName("i_amount")]
    public string? AmountInString { get; set; }
    public long? AmountIn
        => GetLong(AmountInString);

    [JsonPropertyName("o_summ")]
    public string? AmountOutString { get; set; }
    public long? AmountOut
        => GetLong(AmountOutString);

    [JsonPropertyName("i_system")]
    public string? SystemIn { get; set; }
    [JsonPropertyName("o_system")]
    public string? SystemOut { get; set; }

    [JsonPropertyName("o_status")]
    public string? Status { get; set; }

    [JsonPropertyName("i_currency")]
    public string? CurrencyInString { get; set; }
    [JsonPropertyName("ou_currency")]
    public string? CurrencyOutString { get; set; }
    public Currency? Currency
        => GetCurrency(CurrencyInString) ?? GetCurrency(CurrencyOutString);
    public double? Rate
        => GetRate(Currency);

    [JsonPropertyName("i_referer")]
    public string? MethodIn { get; set; }
    [JsonPropertyName("o_method")]
    public string? MethodOut { get; set; }

    //Я не знаю, зачем разработчики апишки вставляют ссылку на картинку
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}
