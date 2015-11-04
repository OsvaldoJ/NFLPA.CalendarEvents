using System.Web.Routing;
using Orchard.CalendarEvents.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Orchard.CalendarEvents.Handlers
{
    public class CalendarPartHandler : ContentHandler
    {
        public CalendarPartHandler(IRepository<CalendarPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<CalendarPart>((context, calendar) =>
            {
                context.Shape.Description = calendar.Description;
            });
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var calendar = context.ContentItem.As<CalendarPart>();

            if (calendar == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.CalendarEvents"},
                {"Controller", "Calendar"},
                {"Action", "Item"},
                {"calendarId", context.ContentItem.Id}
            };
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.CalendarEvents"},
                {"Controller", "CalendarAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.CalendarEvents"},
                {"Controller", "CalendarAdmin"},
                {"Action", "Edit"},
                {"calendarId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.CalendarEvents"},
                {"Controller", "CalendarAdmin"},
                {"Action", "Remove"},
                {"calendarId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.CalendarEvents"},
                {"Controller", "CalendarAdmin"},
                {"Action", "Item"},
                {"calendarId", context.ContentItem.Id}
            };
        }
    }
}