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
    public const Period SmallestPeriod = Period.Hour;
    public static DateTime SeasonTime
        => new(2012, 1, 1);
    public static readonly (DateTime, DateTime) NullTime
        = (new(), new());

    public static DateTime Floor(this DateTime time, Period period = SmallestPeriod)
        => (period switch
        {
            Period.Hour => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0),
            Period.Day => new DateTime(time.Year, time.Month, time.Day, 0, 0, 0),
            Period.Week => new DateTime(time.Year, time.Month, time.Day, 0, 0, 0).AddDays(-(int)time.DayOfWeek),
            Period.Month => new DateTime(time.Year, time.Month, 1, 0, 0, 0),
            Period.HalfYear => new DateTime(time.Year, time.Month <= 6 ? 0 : 7, 1, 0, 0, 0),
            Period.FourYears => new DateTime(time.Year - time.Year % 4, 1, 1, 0, 0, 0),
            _ => time
        });

    public static DateTime Ceiling(this DateTime time, Period period = SmallestPeriod)
        => (period switch
        {
            Period.Hour => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0).AddHours(1),
            Period.Day => new DateTime(time.Year, time.Month, time.Day, 23, 0, 0),
            Period.Week => new DateTime(time.Year, time.Month, time.Day, 23, 0, 0).AddDays(6 - (int)time.DayOfWeek),
            Period.Month => new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 0, 0),
            Period.HalfYear => new DateTime(time.Year, time.Month <= 6 ? 6 : 12, DateTime.DaysInMonth(time.Year, time.Month), 23, 0, 0),
            Period.FourYears => new DateTime(time.Year - time.Year % 4 + 3, 12, 31, 23, 0, 0),
            _ => time
        });

    public static DateTime AddPeriod(this DateTime time, Period period, int count = 1)
        => period switch
        {
            Period.Hour => time.AddHours(1 * count),
            Period.Day => time.AddDays(1 * count),
            Period.Week => time.AddDays(7 * count),
            Period.Month => time.AddMonths(1 * count),
            Period.HalfYear => time.AddMonths(6 * count),
            Period.FourYears => time.AddYears(4 * count),
            _ => time
        };

    public static DateTime ToSeasonTime(this DateTime time, Period season)
        => (season switch
        {
            Period.Day => new DateTime(SeasonTime.Year, SeasonTime.Month, SeasonTime.Day, time.Hour, 0, 0),
            Period.Week => new DateTime(SeasonTime.Year, SeasonTime.Month, (int)time.DayOfWeek + 1),
            Period.Month => new DateTime(SeasonTime.Year, SeasonTime.Month, time.Day),
            Period.HalfYear => new DateTime(SeasonTime.Year, time.Month - (time.Month <= 6 ? 0 : 6), 1),
            Period.FourYears => new DateTime(SeasonTime.Year + time.Year % 4, time.Month, 1),
            _ => SeasonTime
        });

    public static Period GetSeasonDuration(this Period season)
        => season switch
        {
            Period.Hour => Period.Hour,
            Period.Day => Period.Hour,
            Period.Week => Period.Day,
            Period.Month => Period.Day,
            Period.HalfYear => Period.Month,
            Period.FourYears => Period.Month,
            _ => throw new NotImplementedException(),
        };

    public static (DateTime, DateTime) ToInterval(this DateTime time)
        => (time.Ceiling(), time.Ceiling());

    public static ICollection<DateTime> GetTimeCollection(DateTime startTime, DateTime endTime, Period period)
    {
        startTime = startTime.Floor(period);
        endTime = endTime.Ceiling(period);

        List<DateTime> dateList = new();
        for (; startTime <= endTime; startTime = startTime.AddPeriod(period))
            dateList.Add(startTime);
        return dateList;
    }

    public static bool InOnePeriod(this DateTime a, DateTime? b, Period period = SmallestPeriod)
    {
        if (b is null)
            return false;
        return a.Floor(period) == b.Value.Floor(period);
    }
}