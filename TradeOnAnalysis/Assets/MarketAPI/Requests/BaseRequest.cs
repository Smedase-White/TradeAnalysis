using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TradeOnAnalysis.Assets.MarketAPI.Results;

namespace TradeOnAnalysis.Assets.MarketAPI.Requests
{
    public class BaseRequest
    {
        private const string MarketUri = "https://market.csgo.com/api/";
        private readonly static HttpClient MarketClient = new()
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

        public virtual BaseResult? Result
            => DeserializeMessage<BaseResult>(ResultMessage);

        public void Resend() 
            => _requestTask = MarketClient.GetAsync(_requestUri);

        public static T? DeserializeMessage<T>(HttpResponseMessage message) where T : BaseResult
        {
            StreamReader reader = new(message.Content.ReadAsStream());
            string json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
