using Orchard.CalendarEvents.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Orchard.CalendarEvents.Handlers
{
    public class EventPartHandler : ContentHandler
    {
        public EventPartHandler(IRepository<EventPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            //Filters.Add(new ActivatingFilter<CategoryPart>("Site"));
        }
    }
}