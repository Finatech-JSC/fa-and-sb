using System;

namespace MicroBase.Share.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            var startOfWeek = dt.StartOfWeek();
            var endOfWeek = startOfWeek.AddDays(6);

            return endOfWeek;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            var firstDayOfMonth = new DateTime(dt.Year, dt.Month, 1);
            return firstDayOfMonth;
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            var firstDayOfMonth = StartOfMonth(dt);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return lastDayOfMonth;
        }

        public static bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            return StartOfWeek(date1).Date == StartOfWeek(date2).Date && EndOfWeek(date1).Date == EndOfWeek(date2).Date;
        }

        public static bool DatesAreInTheSameMonth(DateTime date1, DateTime date2)
        {
            return date1.Month == date2.Month;
        }

        public static DateTime LongToDateTime(long tick)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(tick);

            return date;
        }

        public static DateTime? UtcToVietnamTime(this DateTime? dt)
        {
            if (dt == null)
            {
                return null;
            }

            return dt.Value.AddHours(7);
        }

        public static DateTime UtcToVietnamTime(this DateTime dt)
        {
            return dt.AddHours(7);
        }
    }
}