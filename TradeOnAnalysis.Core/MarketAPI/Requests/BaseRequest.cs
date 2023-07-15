using System.Text.Json;

namespace TradeOnAnalysis.Core.MarketAPI;

public class BaseRequest<ResultType> where ResultType : BaseResult
{
    private const string MarketUri = "https://market.csgo.com/api/";

    private static readonly HttpClient MarketClient = new()
    {
        BaseAddress = new Uri(MarketUri),
    };

    private readonly string _requestUri;
    private Task<HttpResponseMessage> _requestTask;

    public BaseRequest(string requestUri)
    {
        _requestUri = requestUri;
        _requestTask = MarketClient.GetAsync(_requestUri);
    }

    public BaseRequest(params string[] requestParams)
        : this(string.Join("/", requestParams)) { }

    public HttpResponseMessage ResultMessage
        => _requestTask.Result;

    public virtual ResultType? Result
        => DeserializeMessage(ResultMessage);

    public void Resend()
        => _requestTask = MarketClient.GetAsync(_requestUri);

    public ResultType? DeserializeMessage(HttpResponseMessage message)
    {
        StreamReader reader = new(message.Content.ReadAsStream());
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<ResultType>(json);
    }
}
