using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace EPatrol.Services
{
public class TimeDayConverter
    {
        public static string ConvertTimeAndDay(string timeInput, string dayInput)
        {
            // Parse the time using DateTime
            DateTime parsedTime;
            if (!DateTime.TryParseExact(timeInput, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime) &&
                !DateTime.TryParseExact(timeInput, "h tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime))
            {
                throw new ArgumentException("Invalid time format.");
            }

            // Extract hour and minute
            string hour = parsedTime.Hour.ToString();
            string minute = parsedTime.Minute.ToString();

            // Get the first three characters of the day in uppercase
            if (string.IsNullOrWhiteSpace(dayInput) || dayInput.Length < 3)
            {
                throw new ArgumentException("Invalid day format.");
            }

            string day = dayInput.Substring(0, 3).ToUpper();

            string result_cron_format = "0 " + minute + " " + hour + " ? * " + day + " *";
            // Return as a tuple
            return result_cron_format;
        }

        // cron converter for the time like 12:00, 16:00
        public static string Time24Converter(string timeInput)
        {
            if (string.IsNullOrEmpty(timeInput))
            {
                throw new ArgumentException("Time input cannot be null or empty");
            }

            var timeParts = timeInput.Split(':');
            if (timeParts.Length != 2)
            {
                throw new ArgumentException("Invalid time format. Expected HH:mm");
            }

            if (!int.TryParse(timeParts[0], out int hour) || hour < 0 || hour > 23)
            {
                throw new ArgumentException("Invalid hour value");
            }

            if (!int.TryParse(timeParts[1], out int minute) || minute < 0 || minute > 59)
            {
                throw new ArgumentException("Invalid minute value");
            }

            return $"0 {minute} {hour} 1/1 * ? *";
        }
    }
}