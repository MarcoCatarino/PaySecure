// Helpers/DateTimeHelper.cs
namespace PaySecure.Helpers;

public static class DateTimeHelper
{
    public static string ToFriendlyString(DateTime dateTime)
    {
        var now = DateTime.Now;
        var timeSpan = now - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Ahora mismo";

        if (timeSpan.TotalMinutes < 60)
            return $"Hace {(int)timeSpan.TotalMinutes} minutos";

        if (timeSpan.TotalHours < 24)
            return $"Hace {(int)timeSpan.TotalHours} horas";

        if (timeSpan.TotalDays < 7)
            return $"Hace {(int)timeSpan.TotalDays} días";

        if (timeSpan.TotalDays < 30)
            return $"Hace {(int)(timeSpan.TotalDays / 7)} semanas";

        return dateTime.ToString("dd/MM/yyyy");
    }

    public static string ToMexicanFormat(DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("es-MX"));
    }

    public static bool IsToday(DateTime dateTime)
    {
        return dateTime.Date == DateTime.Now.Date;
    }

    public static bool IsThisWeek(DateTime dateTime)
    {
        var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
        return dateTime >= startOfWeek && dateTime < startOfWeek.AddDays(7);
    }

    public static bool IsThisMonth(DateTime dateTime)
    {
        var now = DateTime.Now;
        return dateTime.Year == now.Year && dateTime.Month == now.Month;
    }
}
