using System;
using System.Collections.Generic;

namespace Orchard.CalendarEvents.Helpers
{
    public enum SupportedTimeZones { Eastern, Mountain, Central, Pacific }


    public class TimeZoneHelper
    {
        public string Tzid { get; set; }
        public string Text { get; set; }

        public static DateTime ConvertTZIDtoUTC(DateTime? date, string tzid)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GetWindowsTimeZoneIdFromIana(tzid));
            var d = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(date), timeZoneInfo);
            return d;
        }
        public static DateTime ConvertTZIDFromUTC(DateTime? date, string tzid)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GetWindowsTimeZoneIdFromIana(tzid));
            var d = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(date), timeZoneInfo);
            return d;
        }
        public static DateTime ConvertTZIDtoUTC(DateTime date, string tzid)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GetWindowsTimeZoneIdFromIana(tzid));
            var d = TimeZoneInfo.ConvertTimeToUtc((date), timeZoneInfo);
            return d;
        }
        public static DateTime ConvertTZIDFromUTC(DateTime date, string tzid)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GetWindowsTimeZoneIdFromIana(tzid));
            var d = TimeZoneInfo.ConvertTimeFromUtc((date), timeZoneInfo);
            return d;
        }

        public static List<TimeZoneHelper> TimeZonesList()
        {
            var list = new List<TimeZoneHelper> {Capacity = 4};
            list.Add(new TimeZoneHelper
            {
                Text = SupportedTimeZones.Central.ToString(),
                Tzid = GetTzid(SupportedTimeZones.Central)
            });
            list.Add(new TimeZoneHelper
            {
                Text = SupportedTimeZones.Eastern.ToString(),
                Tzid = GetTzid(SupportedTimeZones.Eastern)
            });
            list.Add(new TimeZoneHelper
            {
                Text = SupportedTimeZones.Mountain.ToString(),
                Tzid = GetTzid(SupportedTimeZones.Mountain)
            });
            list.Add(new TimeZoneHelper
            {
                Text = SupportedTimeZones.Pacific.ToString(),
                Tzid = GetTzid(SupportedTimeZones.Pacific)
            });
            return list;
        } 

        public static string GetWindowsTimeZoneIdFromIana(string tzid)
        {
            switch (tzid)
            {
                case "America/New_York":
                    return "Eastern Standard Time";
                case "America/Denver":
                    return "Mountain Standard Time";
                case "America/Los_Angeles":
                    return "Pacific Standard Time";
                case "America/Chicago":
                    return "Central Standard Time";
            }
            return "Eastern Standard Time";
        }

        public static SupportedTimeZones GetSupportedTimeZones(string tzid)
        {
            switch (tzid)
            {
                case "America/New_York":
                    return SupportedTimeZones.Eastern;
                case "America/Denver":
                    return SupportedTimeZones.Mountain;
                case "America/Los_Angeles":
                    return SupportedTimeZones.Pacific;
                case "America/Chicago":
                    return SupportedTimeZones.Central;
                case "Eastern Standard Time":
                    return SupportedTimeZones.Eastern;
                case "Mountain Standard Time":
                    return SupportedTimeZones.Mountain;
                case "Pacific Standard Time":
                    return SupportedTimeZones.Pacific;
                case "Central Standard Time":
                    return SupportedTimeZones.Central;
            }
            return SupportedTimeZones.Eastern;
        }

        public static string GetTzid(SupportedTimeZones tz)
        {
            switch (tz)
            {
                case SupportedTimeZones.Eastern:
                    return "America/New_York";
                case SupportedTimeZones.Mountain:
                    return "America/Denver";
                case SupportedTimeZones.Pacific:
                    return "America/Los_Angeles";
                case SupportedTimeZones.Central:
                    return "America/Chicago";
            }
            return "America/New_York";
        }
        public static string GetIana(SupportedTimeZones tz)
        {
            switch (tz)
            {
                case SupportedTimeZones.Eastern:
                    return "Eastern Standard Time";
                case SupportedTimeZones.Mountain:
                    return "Mountain Standard Time";
                case SupportedTimeZones.Pacific:
                    return "Pacific Standard Time";
                case SupportedTimeZones.Central:
                    return "Central Standard Time";
            }
            return "America/New_York";
        }
    }
}