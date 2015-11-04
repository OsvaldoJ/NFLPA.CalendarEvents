using System;
using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class SchedulerViewModel
    {
        public string Identifier { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<CategoryEntry> EventCategories { get; set; }
    }

    public class SchedulerEventViewModel
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Url { get; set; }
        public string Permalink { get; set; }
        public string[] EventCategoryIds { get; set; }
        public string[] EventCategoryNames { get; set; }
        public string Timezone { get; set; }
        public string RecurrenceId { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public string Location { get; set; }
        public bool ImportFromGoogleCalendar { get; set; }
        public string ImportUid { get; set; }
        public bool? CheckDuplicates { get; set; }
    }

}