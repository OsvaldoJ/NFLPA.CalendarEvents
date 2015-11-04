using System;
using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class EventSearchViewModel
    {
        public int CurrentPage { get; set; }
        public int NumberofPages { get; set; }
        public int ResultsPerPage { get; set; }
        public int TotalItems { get; set; }
        public List<SchedulerEventViewModel> SearchResults { get; set; }
    }

    public class EventItemViewModel {
        public EventItemViewModel(EventPart part)
        {
            Identifier = part.Identifier;
            EventCategoryNamesCsv = string.Join(",", part.Categories);
            Title = part.Title;
            StartDate = part.StartDate != null ? Convert.ToDateTime(part.StartDate).ToShortDateString() : string.Empty;
            EndDate = part.EndDate != null ? Convert.ToDateTime(part.EndDate).ToShortDateString() : string.Empty; ;
        }
        public string Identifier { get; set; }
        public string EventCategoryNamesCsv { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}