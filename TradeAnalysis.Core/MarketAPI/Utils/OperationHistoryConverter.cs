using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

public class OperationHistoryConverter : JsonConverter<OperationHistoryBase>
{
    public override OperationHistoryBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? jsonObject = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (jsonObject is null)
            return null;

        Type? type = GetOperationTypeByEvent(GetEvent(jsonObject["h_event"]!.AsValue().ToString()));
        if (type is null)
            return null;

        return jsonObject.Deserialize(type, options) as OperationHistoryBase;
    }

    public override void Write(Utf8JsonWriter writer, OperationHistoryBase value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
