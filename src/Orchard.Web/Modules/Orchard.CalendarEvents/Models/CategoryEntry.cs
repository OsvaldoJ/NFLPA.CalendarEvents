using Orchard.ContentManagement;

namespace Orchard.CalendarEvents.Models
{

    public class CategoryEntry
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public bool Selectable { get; set; }
        public int Count { get; set; }
        public int Weight { get; set; }
        public bool IsChecked { get; set; }
        public ContentItem ContentItem { get; set; }
    }
}