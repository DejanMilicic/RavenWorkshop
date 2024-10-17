using System;
using System.Globalization;

namespace Northwind.Features.Indexes.SalesStats;

public static class Helper
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

    public static decimal CalculateMargin(decimal receivables, decimal payables)
    {
        if (receivables == 0)
        {
            return -100;
        }
        decimal margin = ((receivables - payables) / receivables) * 100;
        return Math.Round(margin, 2);
    }
}
