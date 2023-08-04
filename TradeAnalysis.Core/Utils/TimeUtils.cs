namespace TradeAnalysis.Core.Utils;

public enum Period
{
    Hour,
    Day,
    Week,
    Month,
    HalfYear,
    FourYears
}

public static class TimeUtils
{
    public static DateTime BestTime
        => new(2001, 1, 1);

    public static DateTime GetNow(Period period = Period.Hour)
    {
        return DateTime.Now.RoundByPeriod(period);
    }

    public static ICollection<DateTime> GetRange(DateTime startTime, DateTime endTime, Period period)
    {
        startTime = startTime.RoundByPeriod(period);
        endTime = endTime.RoundByPeriod(period);
        List<DateTime> dateList = new();
        for (; startTime <= endTime; startTime = startTime.AddPeriod(period))
            dateList.Add(startTime);
        return dateList;
    }

    public static bool Equals(this DateTime a, DateTime? b, Period period = Period.Hour)
    {
        if (b is null)
            return false;
        return a.RoundByPeriod(period) == b.Value.RoundByPeriod(period);
    }

    public static DateTime RoundByPeriod(this DateTime time, Period period = Period.Hour)
        => (period switch
        {
            Period.Hour => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0).AddHours(1),
            Period.Day => new DateTime(time.Year, time.Month, time.Day).AddDays(1),
            Period.Week => new DateTime(time.Year, time.Month, time.Day).AddDays(7 - (int)time.DayOfWeek),
            Period.Month => new DateTime(time.Year, time.Month, 0).AddMonths(1),
            Period.HalfYear => new DateTime(time.Year, time.Month <= 5 ? 5 : 11, 0).AddMonths(1),
            Period.FourYears => new DateTime(time.Year - time.Year % 4, 0, 0).AddYears(4),
            _ => throw new ArgumentException("")
        }).AddHours(-1);

    public static DateTime AddPeriod(this DateTime time, Period period, int count = 1)
        => period switch
        {
            Period.Hour => time.AddHours(1 * count),
            Period.Day => time.AddDays(1 * count),
            Period.Week => time.AddDays(7 * count),
            Period.Month => time.AddMonths(1 * count),
            Period.HalfYear => time.AddMonths(6 * count),
            Period.FourYears => time.AddYears(4 * count),
            _ => throw new ArgumentException("")
        };
}