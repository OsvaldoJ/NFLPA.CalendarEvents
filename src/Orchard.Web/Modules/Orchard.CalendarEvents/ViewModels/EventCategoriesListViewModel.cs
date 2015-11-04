using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class EventCategoriesListViewModel
    {
        public IList<CategoryPart> CategoryEntries { get; set; }
        public IList<EventCategoryViewModel> Categories { get; set; } 
    }

    public class EventCategoryViewModel
    {
        public CategoryPart CategoryPart { get; set; }
        public virtual IEnumerable<EventPart> Events { get; set; }
    }
}