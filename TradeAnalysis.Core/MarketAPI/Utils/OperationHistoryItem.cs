using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

public enum Stage
{
    New = 1,
    Given = 2,
    TimedOut = 5
}

public class OperationHistoryItem : OperationHistoryBase
{
    [JsonPropertyName("join")]
    public int? Join { get; set; }

    [JsonPropertyName("app")]
    public string? App { get; set; }

    [JsonPropertyName("id")]
    public string? ItemIdString { get; set; }
    public long? ItemId
        => GetLong(ItemIdString);

    [JsonPropertyName("classid")]
    public string? ClassIdString { get; set; }
    public long? ClassId
        => GetLong(ClassIdString);

    [JsonPropertyName("instanceid")]
    public string? InstanceIdString { get; set; }
    public long? InstanceId
        => GetLong(InstanceIdString);

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("name_color")]
    public string? NameColor { get; set; }

    [JsonPropertyName("market_name")]
    public string? MarketName { get; set; }

    [JsonPropertyName("market_hash_name")]
    public string? MarketHashName { get; set; }

    [JsonPropertyName("paid")]
    public string? PaidString { get; set; }
    public long? Paid
        => GetLong(PaidString);

    [JsonPropertyName("recieved")]
    public string? RecievedString { get; set; }
    public long? Recieved
        => GetLong(RecievedString);

    [JsonPropertyName("stage")]
    public string? StageString { get; set; }
    public Stage Stage
        => (Stage)Convert.ToByte(StageString);

    [JsonPropertyName("item")]
    public string? ItemStrng { get; set; }
    public long? Item
        => GetLong(ItemStrng);

    [JsonPropertyName("flags")]
    public string? Flags { get; set; }

    [JsonPropertyName("l_seller")]
    public string? SellerString { get; set; }
    public long? Seller
        => GetLong(SellerString);

    [JsonPropertyName("l_buyer")]
    public string? BuyerString { get; set; }
    public long? Buyer
        => GetLong(BuyerString);

    [JsonPropertyName("l_seller_currency")]
    public string? SellerCurrencyString { get; set; }
    public Currency? SellerCurrency
        => GetCurrency(SellerCurrencyString);
    public double? SellerRate
        => GetRate(SellerCurrency);

    [JsonPropertyName("l_buyer_currency")]
    public string? BuyerCurrencyString { get; set; }
    public Currency? BuyerCurrency
        => GetCurrency(BuyerCurrencyString);
    public double? BuyerRate
        => GetRate(BuyerCurrency);
}
