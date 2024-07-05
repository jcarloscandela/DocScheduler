using System.Globalization;


namespace DocScheduler.Utils
{
    public static class DateUtils
    {
        public static DateTime GetPreviousMonday(DateTime date)
        {
            int daysUntilMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            return date.Date.AddDays(-daysUntilMonday);
        }

        public static DateTime GetNextMonday(DateTime currentDate)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currentDate.DayOfWeek + 7) % 7;

            return currentDate.AddDays(daysUntilMonday);
        }

        public static string GetFormattedDate(DateTime date)
        {
            return date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}

