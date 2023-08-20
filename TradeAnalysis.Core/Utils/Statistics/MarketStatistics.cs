﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TradeAnalysis.Core.Utils.TimeUtils;

using TradeAnalysis.Core.Utils.MarketItems;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics;

public class MarketStatistics : Statistics<MarketStatisticElement>
{
    private readonly Account _account;

    public MarketStatistics(Account account)
    : base(Period.Hour)
    {
        _account = account;
        CalcStatistics();
    }

    public Account Account
    {
        get => _account;
    }

    public override void CalcStatistics()
    {
        if (_account.ParsedItems is null || _account.ParsedItems.Count == 0)
            return;

        List<DealInfo> allOperations = new();
        foreach (MarketItem item in _account.ParsedItems)
        {
            if (item.History is null)
                continue;
            allOperations.AddRange(item.History);
        }

        DateTime startTime = allOperations.MinBy(o => o.Time)!.Time,
            endTime = allOperations.MaxBy(o => o.Time)!.Time;

        Data = new(from time in GetTimeCollection(startTime, endTime, Period.Hour)
                   select new MarketStatisticElement() { Time = time });

        FillStatisticValues(allOperations,
            item => item.Time.ToInterval(),
            (item, _) => item.Amount,
            (data, value) => data.Price = value,
            values => values.Average());

        FillStatisticValues(allOperations,
            item => item.Time.ToInterval(),
            (item, _) => 0,
            (data, value) => data.Count = value,
            values => values.Count());
    }
}
