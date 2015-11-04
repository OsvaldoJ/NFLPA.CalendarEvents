using Orchard.ContentManagement;

namespace Orchard.CalendarEvents.Models
{
    public class CalendarWidgetPart : ContentPart
    {
        public string CalendarIdentifier
        {
            get { return this.Retrieve(x => x.CalendarIdentifier); }
            set { this.Store(x => x.CalendarIdentifier, value); }
        }

        public int Count
        {
            get
            {
                var count = this.Retrieve(x => x.Count);
                return count == 0 ? 5 : count;
            }
            set { this.Store(x => x.Count, value); }
        }
    }
}