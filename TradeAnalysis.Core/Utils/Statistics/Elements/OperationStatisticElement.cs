using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    public double Buy { get; set; } = 0;
    public double Sell { get; set; } = 0;

    public override void Combine(CombineType combineType, IEnumerable<StatisticElement> elements)
    {
        Combine(combineType, (elements as IEnumerable<OperationStatisticElement>)!);
    }

    public virtual void Combine(CombineType combineType, IEnumerable<OperationStatisticElement> elements)
    {
        base.Combine(combineType, elements);
        int count = elements.Count() + 1;
        switch (combineType)
        {
            case CombineType.Sum:
                Buy += elements.Sum(e => e.Buy);
                Sell += elements.Sum(e => e.Sell);
                break;
            case CombineType.Average:
                Buy = (Buy + elements.Sum(e => e.Buy)) / count;
                Sell = (Sell + elements.Sum(e => e.Sell)) / count;
                break;
            default:
                break;
        }
    }

    public virtual void Combine(CombineType combineType, params OperationStatisticElement[] elements)
        => Combine(combineType, elements as IEnumerable<OperationStatisticElement>);
}
