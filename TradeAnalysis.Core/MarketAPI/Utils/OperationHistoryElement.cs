using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

public enum EventType
{
    Buy,
    Sell,
    Transaction
}

public enum TradeStage
{
    New = 1,
    Given = 2,
    TimedOut = 5
}

public class OperationHistoryElement
{
    [JsonPropertyName("h_id")]
    public string? HistoryID { get; set; }

    [JsonPropertyName("h_event")]
    public string? EventString { get; set; }
    public EventType Event
        => EventString switch
        {
            "buy_go" => EventType.Buy,
            "sell_go" => EventType.Sell,
            _ => EventType.Transaction
        };

    [JsonPropertyName("h_time")]
    public string? TimeString { get; set; }
    public DateTime Time
        => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(TimeString)).LocalDateTime;

    [JsonPropertyName("h_event_id")]
    public string? EventID { get; set; }

    [JsonPropertyName("join")]
    public int? Join { get; set; }

    [JsonPropertyName("app")]
    public string? App { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("classid")]
    public string? ClassId { get; set; }

    [JsonPropertyName("instanceid")]
    public string? InstanceId { get; set; }

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("name_color")]
    public string? NameColor { get; set; }

    [JsonPropertyName("market_name")]
    public string? MarketName { get; set; }

    [JsonPropertyName("market_hash_name")]
    public string? MarketHashName { get; set; }

    [JsonPropertyName("paid")]
    public string? Paid { get; set; }

    [JsonPropertyName("recieved")]
    public string? Recieved { get; set; }

    [JsonPropertyName("stage")]
    public string? StageString { get; set; }
    public TradeStage Stage
        => (TradeStage)Convert.ToByte(StageString);

    [JsonPropertyName("item")]
    public string? Item { get; set; }

    [JsonPropertyName("flags")]
    public string? Flags { get; set; }
}