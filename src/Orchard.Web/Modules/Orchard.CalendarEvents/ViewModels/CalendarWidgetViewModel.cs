using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class CalendarWidgetViewModel
    {
        public string SelectedCalendarIdentifier { get; set; }
        public int Count { get; set; }
        public IList<CalendarPart> Calendars { get; set; }
    }
}