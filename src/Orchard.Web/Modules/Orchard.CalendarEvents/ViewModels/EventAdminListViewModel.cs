using System.Collections.Generic;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class EventAdminListViewModel
    {
        public IList<EventPart> Items { get; set; }
        public IList<CategoryPart> CategoriesList { get; set; }
        public string CalendarId { get; set; }
        public CalendarPart Calendar { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}