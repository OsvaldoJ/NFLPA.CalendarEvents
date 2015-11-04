using Orchard.CalendarEvents.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Orchard.CalendarEvents.Handlers
{
    public class CategoryPartHandler : ContentHandler
    {
        public CategoryPartHandler(IRepository<CategoryPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            //Filters.Add(new ActivatingFilter<CategoryPart>("Site"));
        }
    }
}