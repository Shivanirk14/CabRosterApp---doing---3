// BookingService.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace CabRosterApp.Services
{
    public static class BookingService
    {
        public static List<DateTime> GetAvailableWeekdays(DateTime today, int weeks)
        {
            var startDate = GetStartOfNextWeek(today);
            var endDate = startDate.AddDays(5);  // For Monday to Friday range

            var availableDates = new List<DateTime>();

            for (int i = 0; i < 5; i++)
            {
                availableDates.Add(startDate.AddDays(i));
            }

            return availableDates;
        }

        public static List<DateTime> GetWeekdaysBetween(DateTime startDate, DateTime endDate)
        {
            var weekdays = new List<DateTime>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Add Monday to Friday and skip weekends
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekdays.Add(date);
                }
            }

            return weekdays;
        }

        private static DateTime GetStartOfNextWeek(DateTime current)
        {
            // Ensure we calculate the start of the next Monday
            var daysToNextMonday = DayOfWeek.Monday - current.DayOfWeek;
            daysToNextMonday = daysToNextMonday == 0 ? 7 : daysToNextMonday + 7;

            return current.AddDays(daysToNextMonday);
        }
    }
}
