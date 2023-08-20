using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

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
    public DateTime? Time
        => GetTime(TimeString);
}
