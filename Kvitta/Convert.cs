namespace Kvitta;

public static class Convert
{
    public static DateOnly ToDateOnly(this DateTimeOffset x) => new(x.Year, x.Month, x.Day);
    public static DateOnly ToDateOnly(this DateTime x) => new(x.Year, x.Month, x.Day);
}