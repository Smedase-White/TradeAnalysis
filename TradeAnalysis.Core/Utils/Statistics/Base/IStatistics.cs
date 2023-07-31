using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public interface IStatistics
{
    public void CalcStatistics();

    public static void CalcStatisticValues<StatisticType, ItemsType>(
        IEnumerable<StatisticType> data, IEnumerable<ItemsType> items,
        Func<ItemsType, DateTime, bool> predicate, Func<ItemsType, DateTime, double> selection,
        Action<StatisticType, double> action, Func<IEnumerable<double>, double>? calc = null)
        where StatisticType : StatisticElement, new()
    {
        foreach (StatisticType element in data)
        {
            IEnumerable<double> values = from item in items
                                         where predicate(item, element.Time)
                                         select selection(item, element.Time);
            double value = calc is null ? values.Sum() : calc(values);
            action(element, value);
        }
    }
}
