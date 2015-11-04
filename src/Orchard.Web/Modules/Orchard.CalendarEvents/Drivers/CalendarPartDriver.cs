using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Orchard.CalendarEvents.Drivers
{
    [UsedImplicitly]
    public class CalendarPartDriver : ContentPartDriver<CalendarPart>
    {
        private const string TemplateName = "Parts/Calendar";
        private readonly ICategoryService _eventCategoryService;
        private readonly ICalendarService _calendarService;
        private readonly ICategoryService _categoryService;

        public CalendarPartDriver(
            ICategoryService eventCategoryService,
            ICalendarService calendarService, 
            ICategoryService categoryService)
        {
            _eventCategoryService = eventCategoryService;
            _calendarService = calendarService;
            _categoryService = categoryService;
        }

        protected override string Prefix
        {
            get { return "Calendar"; }
        }

        protected override DriverResult Display(CalendarPart part, string displayType, dynamic shapeHelper)
        {
            if (displayType == "SummaryAdmin")
            {
                return ContentShape("Parts_Calendar_SummaryAdmin", 
                    () => shapeHelper.Parts_Calendar_SummaryAdmin(
                    ContentPart: part
                ));
            }

            if (displayType == "DetailAdmin")
            {
                return ContentShape("Parts_Calendar_DetailAdmin",
                    () => shapeHelper.Parts_Calendar_DetailAdmin(
                    SchedulerViewModel: _calendarService.CreateSchedulerViewModel(part),
                    CalendarPart: part
                ));
            }

            var eventCategories = _eventCategoryService.GetEventCategoriesByCalendar(part.ContentItem);
            //var events = _calendarService.GetEvents(part.As<IdentityPart>().Identifier);// _eventService.GetEventsForCalendar(part);


            return ContentShape("Parts_Calendar_Detail",
                            () => shapeHelper.Parts_Calendar_Detail(
                                //Events: events,
                                EventCategories:eventCategories,
                                ContentPart: part
                                ));
        }

        protected override DriverResult Editor(CalendarPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Calendar_Edit",
                                () => shapeHelper
                                    .EditorTemplate(
                                        TemplateName: TemplateName,
                                        Model: _calendarService.CreateEditViewModel(part), 
                                        Prefix: Prefix
                                    ));

        }

        protected override DriverResult Editor(CalendarPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = _calendarService.CreateEditViewModel(part);
            updater.TryUpdateModel(model, Prefix, null, null);
            if(part.ContentItem.Id != 0)
            {
                _calendarService.UpdateCalendarForContentItem(part.ContentItem, model);
            }

            return ContentShape("Parts_Calendar_Edit",
                                    () => shapeHelper
                                        .EditorTemplate(
                                            TemplateName: TemplateName,
                                            Model: model,
                                            Prefix: Prefix
                                        ));
        }


        
        protected override void Exporting(CalendarPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.Title);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ShortDescription", part.ShortDescription);
            context.Element(part.PartDefinition.Name).SetAttributeValue("FeedProxyUrl", part.FeedProxyUrl);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EventCategoryIdsValue", string.Join(",", part.Categories.Select(x => x.Id)));
        }

        protected override void Importing(CalendarPart part, ImportContentContext context)
        {
            foreach (
                var prop in
                    part.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name))
            {
                var data = context.Attribute(part.PartDefinition.Name, prop);
                if (data == null) continue;
                switch (prop)
                {
                    case "EventCategoryIdsValue":
                        _categoryService.UpdateCategoriesForCalendar(part.ContentItem, data.Split(',').Select(x => new CategoryEntry { Id = int.Parse(x), IsChecked = true }));
                        break;
                    case "Description":
                        part.Description = data;
                        break;
                    case "ShortDescription":
                        part.ShortDescription = data;
                        break;
                    case "FeedProxyUrl":
                        part.FeedProxyUrl = data;
                        break;
                }
            }
        }
    }
}