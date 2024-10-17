using System;
using System.Globalization;

namespace Northwind.Features.Indexes.SalesStats;

public static class DateHelper
{
    public static int GetIso8601WeekNumber(DateTime date)
    {
        // Create a Gregorian calendar object
        Calendar calendar = CultureInfo.InvariantCulture.Calendar;

        // Get the day of the week for the specified date
        DayOfWeek day = calendar.GetDayOfWeek(date);

        // Adjust the date to Thursday in the same week, ensuring correct week number calculation
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
        }

        // Return the ISO 8601 week number
        return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    public static int GetQuarterNumber(DateTime date)
    {
        // Calculate the quarter number from the month
        return (date.Month + 2) / 3;
    }
}
