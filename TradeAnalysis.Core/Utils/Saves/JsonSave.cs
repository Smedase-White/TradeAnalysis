using System.Text.Json;

namespace TradeAnalysis.Core.Utils.Saves;

public static class JsonSave
{
    private static readonly JsonSerializerOptions _options
        = new() { WriteIndented = true };

    public static T? Load<T>(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, _options);
    }

    public static void Save<T>(T obj, string path)
    {
        using FileStream stream = File.Create(path);
        JsonSerializer.Serialize(stream, obj, _options);
    }
}
