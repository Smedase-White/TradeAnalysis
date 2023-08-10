using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class MarketStatisticElement : StatisticElement
{
    private double _price = 1;

    public double Price
    {
        get => _price;
        set => _price = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Price == 1);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Average(ref _price, elements, e => (e as MarketStatisticElement)!.Price);
    }
}
