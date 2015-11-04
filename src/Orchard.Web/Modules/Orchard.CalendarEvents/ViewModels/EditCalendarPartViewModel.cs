using System.Collections.Generic;
using System.ComponentModel;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class EditCalendarPartViewModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string FeedProxyUrl { get; set; }
        [DisplayName("Event Categories Included")]
        public string SelectedEventCategoryIds { get; set; }
        public List<CategoryEntry> Categories { get; set; }
    }
}