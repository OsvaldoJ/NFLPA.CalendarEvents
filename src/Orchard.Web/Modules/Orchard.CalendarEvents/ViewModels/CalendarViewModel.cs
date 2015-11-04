using System;
using System.Collections.Generic;

namespace Orchard.CalendarEvents.ViewModels
{
    public class CalendarViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public List<EventIcs> Events { get; set; }
        public string CategoriesCsv { get; set; }
    }

    
    public class EventIcs
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Url { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string CategoriesCsv { get; set; }
        public string LocationName { get; set; }
        public string TimeZone { get; set; }
        public string RecurrenceId { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
    }
}