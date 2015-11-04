using System;
using System.Linq;
using System.Reflection;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.CalendarEvents.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Orchard.CalendarEvents.Drivers
{
    public class CalendarWidgetPartDriver : ContentPartDriver<CalendarWidgetPart>
    {
        private readonly ICalendarService _calendarService;
        private readonly IEventService _eventService;

        public CalendarWidgetPartDriver(
            ICalendarService calendarService,
            IEventService eventService)
        {
            _calendarService = calendarService;
            _eventService = eventService;
        }


        protected override DriverResult Display(CalendarWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_CalendarWidget",
                () => {
                    var calendar = _calendarService.GetCalendar(part.CalendarIdentifier, VersionOptions.Published);
                    if (calendar == null) {
                        return null;
                    }
                    int count = part.Count <= 0 ? 5 : part.Count;
                    var events = _eventService.GetUpcomingEventsForCalendar(calendar, VersionOptions.Latest, count);
                    return shapeHelper.Parts_CalendarWidget(
                        EventsList: events,
                        Calendar: calendar
                        );
                });
        }

        protected override DriverResult Editor(CalendarWidgetPart part, dynamic shapeHelper)
        {
            var viewModel = new CalendarWidgetViewModel
            {
                Count = part.Count,
                SelectedCalendarIdentifier = part.CalendarIdentifier,
                Calendars = _calendarService.Get().OrderBy(b => b.Title).ToList(),
            };

            return ContentShape("Parts_CalendarWidget_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/CalendarWidget", 
                    Model: viewModel, 
                    Prefix: Prefix
                    ));
        }

        protected override DriverResult Editor(CalendarWidgetPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new CalendarWidgetViewModel();

            if (updater.TryUpdateModel(viewModel, Prefix, null, null))
            {
                part.CalendarIdentifier = viewModel.SelectedCalendarIdentifier;
                part.Count = viewModel.Count;
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(CalendarWidgetPart part, ImportContentContext context)
        {
            foreach (
                var prop in
                    part.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name))
            {
                var data = context.Attribute(part.PartDefinition.Name, prop);
                if (data == null) continue;
                switch (prop)
                {
                    case "CalendarIdentifier":
                        part.CalendarIdentifier = data;
                        break;
                    case "Count":
                        part.Count = Int32.Parse(data);
                        break;
                }
            }
        }

        protected override void Exporting(CalendarWidgetPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarIdentifier", part.CalendarIdentifier);

            context.Element(part.PartDefinition.Name).SetAttributeValue("Count", part.Count);
        }
    }
}