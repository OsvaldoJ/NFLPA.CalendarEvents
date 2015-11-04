using Orchard.ContentManagement.Records;

namespace Orchard.CalendarEvents.Models
{
    public class CalendarCategoryJoinRecord
    {
        public virtual int Id { get; set; }
        public virtual CategoryPartRecord CategoryPartRecord { get; set; }
        public virtual CalendarPartRecord CalendarPartRecord { get; set; }
    }
}