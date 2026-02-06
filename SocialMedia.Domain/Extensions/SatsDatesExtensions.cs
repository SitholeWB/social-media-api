namespace SocialMedia.Domain;

public static class SatsDatesExtensions
{
    public static DateTimeOffset ToWeekStartDate(this DateTimeOffset dateTime)
    {
        var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
        var date = dateTime.AddDays(-1 * diff);
        return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeOffset.UtcNow.Offset);
    }

    public static DateTimeOffset ToMonthStartDate(this DateTimeOffset dateTime)
    {
        return new DateTimeOffset(dateTime.Year, dateTime.Month, 1, 0, 0, 0, DateTimeOffset.UtcNow.Offset);
    }
}