using Orchard.ContentManagement.Records;

namespace Orchard.CalendarEvents.Models
{
    public class EventCategoryJoinRecord 
    {
        public virtual int Id { get; set; }
        public virtual CategoryPartRecord CategoryPartRecord { get; set; }
        public virtual EventPartRecord EventPartRecord { get; set; } 
    }
}