namespace TradeAnalysis.Core.Utils;

public enum Period
{
    Hour,
    Day,
    Week,
    Month,
    Year,
}

public static class TimeUtils
{
    public const Period SmallestPeriod = Period.Hour;
    public static DateTime SeasonTime
        => new(2012, 1, 1);
    public static readonly (DateTime, DateTime) NullTime
        = (new(), new());

    public static DateTime Floor(this DateTime time, Period period = SmallestPeriod)
        => (period switch
        {
            Period.Hour => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0),
            Period.Day => new DateTime(time.Year, time.Month, time.Day),
            Period.Week => new DateTime(time.Year, time.Month, time.Day).AddDays(-(int)time.DayOfWeek),
            Period.Month => new DateTime(time.Year, time.Month, 1),
            Period.Year => new DateTime(time.Year, 1, 1),
            _ => time
        });

    public static DateTime Ceiling(this DateTime time, Period period = SmallestPeriod)
        => (period switch
        {
            Period.Hour => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0).AddHours(1),
            Period.Day => new DateTime(time.Year, time.Month, time.Day, 23, 0, 0),
            Period.Week => new DateTime(time.Year, time.Month, time.Day, 23, 0, 0).AddDays(6 - (int)time.DayOfWeek),
            Period.Month => new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 0, 0),
            Period.Year => new DateTime(time.Year, 12, 31, 23, 0, 0),
            _ => time
        });

    public static DateTime AddPeriod(this DateTime time, Period period, int count = 1)
        => period switch
        {
            Period.Hour => time.AddHours(count),
            Period.Day => time.AddDays(count),
            Period.Week => time.AddDays(7 * count),
            Period.Month => time.AddMonths(count),
            Period.Year => time.AddYears(count),
            _ => time
        };

    public static DateTime ToSeasonTime(this DateTime time, Period season)
        => (season switch
        {
            Period.Day => new DateTime(SeasonTime.Year, SeasonTime.Month, SeasonTime.Day, time.Hour, 0, 0),
            Period.Week => new DateTime(SeasonTime.Year, SeasonTime.Month, (int)time.DayOfWeek + 1),
            Period.Month => new DateTime(SeasonTime.Year, SeasonTime.Month, time.Day),
            Period.Year => new DateTime(SeasonTime.Year, time.Month, 15),
            _ => SeasonTime
        });

    public static Period GetSeasonDuration(this Period season)
        => season switch
        {
            Period.Hour => Period.Hour,
            Period.Day => Period.Hour,
            Period.Week => Period.Day,
            Period.Month => Period.Day,
            Period.Year => Period.Month,
            _ => throw new NotImplementedException(),
        };

    public static (DateTime, DateTime) ToInterval(this DateTime time)
        => (time.Ceiling(), time.Ceiling());

    public static IEnumerable<DateTime> GetTimeEnumerable(DateTime startTime, DateTime endTime, Period period)
    {
        startTime = startTime.Floor(period);
        endTime = endTime.Ceiling(period);

        for (DateTime time = startTime; time <= endTime; time = time.AddPeriod(period))
            yield return time;
    }

    public static bool InOnePeriod(this DateTime a, DateTime? b, Period period = SmallestPeriod)
    {
        if (b is null)
            return false;
        return a.Floor(period) == b.Value.Floor(period);
    }

    public static double Length(this TimeSpan timeSpan, Period period = SmallestPeriod)
        => period switch
        {
            Period.Hour => timeSpan.TotalHours,
            Period.Day => timeSpan.TotalDays,
            Period.Week => timeSpan.TotalDays / 7,
            Period.Month => timeSpan.TotalDays / 30,
            Period.Year => timeSpan.TotalDays / 365,
            _ => throw new NotImplementedException(),
        };

    public static DateTime Min(params DateTime?[] times)
    {
        DateTime min = DateTime.MaxValue;
        foreach (DateTime? time in times)
            if (time < min) min = time.Value;
        return min;
    }

    public static DateTime Max(params DateTime?[] times)
    {
        DateTime max = DateTime.MinValue;
        foreach (DateTime? time in times)
            if (time > max) max = time.Value;
        return max;
    }

}