namespace Ofichina.Contracts.Extension;

public static class DateTimeExtensions
{
    public static string ToDateString(
        this DateTime date,
        string format = "dd/MM/yyyy")
    {
        return date.ToString(format);
    }

    public static string? ToDateString(
        this DateTime? date,
        string format = "dd/MM/yyyy")
    {
        return date?.ToString(format);
    }

    public static string ToDateString(
        this DateOnly date,
        string format = "dd/MM/yyyy")
    {
        return date.ToString(format);
    }

    public static string? ToDateString(
        this DateOnly? date,
        string format = "dd/MM/yyyy")
    {
        return date?.ToString(format);
    }
}