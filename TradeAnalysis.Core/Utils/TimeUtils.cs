using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAnalysis.Core.Utils;

public static class TimeUtils
{
    public static IEnumerable<DateTime> HoursInRangeUntil(DateTime startTime, DateTime endTime)
    {
        return Enumerable.Range(0, 1 + (int)(endTime - startTime).TotalHours)
                         .Select(time => startTime.AddHours(time));
    }

    public static IEnumerable<DateTime> DaysInRangeUntil(DateTime startTime, DateTime endTime)
    {
        return Enumerable.Range(0, 1 + (int)(endTime - startTime).TotalDays)
                         .Select(time => startTime.AddDays(time));
    }

    public static bool TimeByHourEquals(DateTime? a, DateTime? b)
    {
        if (a is null || b is null)
            return false;
        return a.Value.Year == b.Value.Year && a.Value.Month == b.Value.Month
            && a.Value.Day == b.Value.Day && a.Value.Hour == b.Value.Hour;
    }
}
