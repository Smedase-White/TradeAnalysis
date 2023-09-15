using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;

namespace TradeAnalysis.Core.Utils.MarketItems;

public class MarketItem
{
    private static readonly string[] IgnoredItems = { "Sealed Graffiti" };

    private const int MaxParseTries = 5;
    private const double PeakValue = 1.15;

    private readonly long _classId;
    private readonly long _instanceId;
    private readonly string _name;

    private DealInfo? _buyInfo;
    private DealInfo? _sellInfo;
    private ProfitInfo? _profit;

    private IImmutableList<DealInfo>? _history;
    private double? _averagePrice;

    public MarketItem(long classId, long instanceId, string name)
    {
        _classId = classId;
        _instanceId = instanceId;
        _name = name;
    }

    public MarketItem(OperationHistoryItem element)
        : this(element.ClassId ?? 0, element.InstanceId ?? 0, element.MarketHashName ?? "-")
    {
        switch (element.Event)
        {
            case EventType.Buy: BuyInfo = new(element); break;
            case EventType.Sell: SellInfo = new(element); break;
        }
    }

    public MarketItem(MarketItem item)
    {
        _classId = item.ClassId;
        _instanceId = item.InstanceId;
        _name = item.Name;
        _buyInfo = item.BuyInfo;
        _sellInfo = item.SellInfo;
        _profit = item.Profit;
        _averagePrice = item.AveragePrice;
        _history = item.History;
    }

    public long ClassId
    {
        get => _classId;
    }

    public long InstanceId
    {
        get => _instanceId;
    }

    public string Name
    {
        get => _name;
    }

    public DealInfo? BuyInfo
    {
        get => _buyInfo;
        set
        {
            _buyInfo = value;
            if (_sellInfo is not null)
                Profit = new(_buyInfo!, _sellInfo!);
        }
    }

    public DealInfo? SellInfo
    {
        get => _sellInfo;
        set
        {
            _sellInfo = value;
            if (_buyInfo is not null)
                Profit = new(_buyInfo!, _sellInfo!);
        }
    }

    public ProfitInfo? Profit
    {
        get => _profit;
        private set => _profit = value;
    }

    public IImmutableList<DealInfo>? History
    {
        get => _history;
        private set => _history = value;
    }

    public double? AveragePrice
    {
        get => _averagePrice;
        private set => _averagePrice = value;
    }

    public HttpStatusCode LoadHistory(string apiKey)
    {
        if (ClassId == 0 || InstanceId == 0)
            return HttpStatusCode.BadRequest;

        ItemHistoryRequest request = new(ClassId, InstanceId, apiKey);
        HttpStatusCode status = request.ResultMessage.StatusCode;
        int tries = 1;
        while (status != HttpStatusCode.OK)
        {
            request = new(ClassId, InstanceId, apiKey);
            status = request.ResultMessage.StatusCode;
            if (tries++ == MaxParseTries)
                return status;
        }

        ItemHistoryResult result = request.Result!;

        AveragePrice = Convert.ToDouble(result.Average);

        List<DealInfo> history = new();
        foreach (ItemHistoryElement historyElement in result.History)
            history.Add(new(historyElement.Time!.Value, historyElement.Price / AveragePrice ?? 0));

        for (int i = 1; i < history.Count - 1; i++)
        {
            double near = (history[i - 1].Amount + history[i + 1].Amount) / 2;
            if (history[i].Amount < near / PeakValue || history[i].Amount > near * PeakValue)
            {
                history.RemoveAt(i);
                i--;
            }
        }

        History = history.ToImmutableList();

        return status;
    }

    public bool IsIgnored()
    {
        foreach (string ignoredItem in IgnoredItems)
            if (Name.Contains(ignoredItem))
                return true;
        return false;
    }
}