using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class CalendarAdminListViewModel
    {
        public IList<CalendarPart> Calendars { get; set; }
        public IList<CategoryPart> CategoriesList { get; set; } 
    }
}