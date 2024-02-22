using System.Text.Json;

namespace TradeAnalysis.Core.Utils.Saves;

public static class JsonSave
{
    private static readonly JsonSerializerOptions _options
        = new() { WriteIndented = true };

    public static T? Load<T>(string path)
    {
        if (File.Exists(path) == false)
            return default;
        try
        {
            using FileStream stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<T>(stream, _options);
        }
        catch
        {
            return default;
        }
    }

    public static void Save<T>(T obj, string path)
    {
        try
        {
            using FileStream stream = File.Create(path);
            JsonSerializer.Serialize(stream, obj, _options);
        }
        catch { }
    }
}
