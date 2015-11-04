using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;

namespace Orchard.CalendarEvents.Services
{
    public interface ICalendarService : IDependency
    {
        ContentItem Get(string id, VersionOptions versionOptions);
        CalendarPart GetCalendar(string id, VersionOptions versionOptions);
        IEnumerable<CalendarPart> Get();
        IEnumerable<CalendarPart> Get(VersionOptions versionOptions);
        void Delete(ContentItem calendar);
        IEnumerable<EventPart> GetEvents(string calendarId);
        void UpdateCalendarForContentItem(ContentItem item, EditCalendarPartViewModel model);
        CalendarViewModel CreateCalendarViewModel(string calendarId);
        SchedulerViewModel CreateSchedulerViewModel(CalendarPart calendar);
        IEnumerable<SchedulerEventViewModel> SchedulerEventsForCalendar(CalendarPart calendar);
        EditCalendarPartViewModel CreateEditViewModel(CalendarPart part);
        void UpdateCalendarForSchedulerEventViewModel(ContentItem item, SchedulerEventViewModel model);
    }

    public class CalendarService : ICalendarService
    {
        private readonly IContentManager _contentManager;
        private readonly ICategoryService _categoryService;
        private readonly IEventService _eventService;

        public CalendarService(IContentManager contentManager,
            ICategoryService categoryService, 
            IEventService eventService)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _contentManager = contentManager;
        }
        
        public ContentItem Get(string id, VersionOptions versionOptions)
        {
            int recordId;
            var isRecordId = int.TryParse(id, out recordId);
            if (isRecordId)
            {
                return _contentManager.Get(recordId, VersionOptions.Published);
            }
            var itemPart = GetCalendar(id,versionOptions);
            return itemPart == null ? null : itemPart.ContentItem;
        }
        public CalendarPart GetCalendar(string id, VersionOptions versionOptions)
        {
            var itemPart = _contentManager.Query<IdentityPart, IdentityPartRecord>(versionOptions ?? VersionOptions.Latest)
                .Where(i => i.Identifier.Equals(id, StringComparison.OrdinalIgnoreCase))
                .Join<IdentityPartRecord>()
                .Join<CalendarPartRecord>()
                .List<CalendarPart>()
                .SingleOrDefault();

            return itemPart;
        }

        public IEnumerable<CalendarPart> Get()
        {
            return Get(VersionOptions.Latest);
        }

        public IEnumerable<CalendarPart> Get(VersionOptions versionOptions)
        {
            return _contentManager
                .Query<CalendarPart>(versionOptions ?? VersionOptions.Latest)
                .Join<CalendarPartRecord>()
                .List<CalendarPart>()
                .OrderBy(br => br.Title);
        }

        public void Delete(ContentItem calendar)
        {
            _contentManager.Remove(calendar);
        }

        public IEnumerable<EventPart> GetEvents(string calendarId)
        {
            //Get events by getting all EventCategories from calendar, that match.
            var calendarItem = Get(calendarId, VersionOptions.Published).As<CalendarPart>();
            return _eventService.GetEventsForCalendar(calendarItem, VersionOptions.Latest);
        }

        public void UpdateCalendarForContentItem(
            ContentItem item,
            EditCalendarPartViewModel model)
        {

            var calendarPart = item.As<CalendarPart>();
            //calendarPart.Title = model.Title;
            calendarPart.Description = model.Description;
            calendarPart.ShortDescription = model.ShortDescription;
            calendarPart.FeedProxyUrl = model.FeedProxyUrl;

            _categoryService.UpdateCategoriesForCalendar(item, model.SelectedEventCategoryIds.Split(','));

        }

        public CalendarViewModel CreateCalendarViewModel(string calendarId)
        {
            var calendar = Get(calendarId, VersionOptions.Published)
                .As<CalendarPart>();
            var ics = new CalendarViewModel
            {
                Title = calendar.Title,
                Summary = calendar.ShortDescription,
                CreatedDateTime = Convert.ToDateTime(calendar.CreateUtc),
                UpdatedDateTime = Convert.ToDateTime(calendar.ModifiedUtc),
                CategoriesCsv = string.Join(",", _categoryService
                                .GetEventCategoriesByCalendar(calendar.ContentItem)
                                .Select(y=>y.CategoryName)),
                Events = _eventService
                    .GetEventsForCalendar(calendar, VersionOptions.Published)
                    .Select(_eventService.GetEventIcs)
                    .ToList(),
            };
            return ics;
        }

        public SchedulerViewModel CreateSchedulerViewModel(CalendarPart calendar)
        {
            var eventCategories = _categoryService
                .GetEventCategoriesByCalendar(calendar.ContentItem);

            var data = new SchedulerViewModel
            {
                Identifier = calendar.Identifier,
                Title = calendar.Title,
                Summary = calendar.ShortDescription,
                EventCategories = eventCategories.Select(x=> new CategoryEntry
                {
                    Identifier = x.Identifier,
                    Name = x.CategoryName,
                    Description = x.Description,
                    IsChecked = calendar.Categories.Any(cat => cat.Id == x.Id),

                }).ToList(),
            };
            return data;
        }

        public IEnumerable<SchedulerEventViewModel> SchedulerEventsForCalendar(CalendarPart calendar)
        {
            var events = _eventService.SchedulerEventsForCalendarIdentity(calendar.Identifier, null);
            return events;
        }

        public void UpdateCalendarForSchedulerEventViewModel(ContentItem item, SchedulerEventViewModel model)
        {
            //var categories = _eventCategoryService.GetEventCategoriesByIdList(model.EventCategoryIds).ToList();

            var eventPart = item.As<EventPart>();

            if (model.EventCategoryIds != null && model.EventCategoryIds.Any())
            {
                _categoryService.UpdateCategoriesForEvent(item, model.EventCategoryIds);
            }

            eventPart.As<TitlePart>().Title = model.Title.Replace(@"\,",",");
            eventPart.RecurrenceId = model.RecurrenceId;
            eventPart.RecurrenceRule = model.RecurrenceRule;
            eventPart.RecurrenceException = model.RecurrenceException;
            eventPart.TimeZone = model.Timezone;
            eventPart.Description = model.Description.Replace(@"\,", ",");
            eventPart.AddressLocation = model.Location.Replace(@"\,", ",");
            eventPart.Url = model.Url;
            eventPart.ImportedFromGoogleCalendar = model.ImportFromGoogleCalendar;
            eventPart.ImportUid = model.ImportUid;

            eventPart.AllDayEvent = model.IsAllDay;

            if (model.IsAllDay && model.ImportFromGoogleCalendar)
            {
                model.End = model.End.AddDays(-1); // catch for import 
            }

            eventPart.StartDate = model.Start;
            eventPart.EndDate = model.End;
        }


        public EditCalendarPartViewModel CreateEditViewModel(CalendarPart part)
        {
            //var settings = part.PartDefinition.Settings.GetModel<SharedEventCategorySettings>();
            var data = new EditCalendarPartViewModel
            {
                Title = part.Title,
                Description = part.Description,
                ShortDescription = part.ShortDescription,
                FeedProxyUrl = part.FeedProxyUrl,
                Categories = GetCategoryEntries(part),
                SelectedEventCategoryIds = string.Join(",", _categoryService.GetEventCategoryIdentifiers(part.Categories.Select(x => x.Id))),
            };
            return data;
        }

        private List<CategoryEntry> GetCategoryEntries(CalendarPart part)
        {
            var categories = _categoryService.GetEventCategories();

            var data = categories.Select(c => new CategoryEntry
            {
                ContentItem = c.ContentItem,
                Identifier = c.Identifier,
                Id = c.Id,
                IsChecked = IsChecked(part, c.Identifier),
                Selectable = true,
                Name = c.CategoryName
            }).ToList();

            return data;
        }
        private bool IsChecked(CalendarPart part, string id)
        {
            var categoryIdentifiers = _categoryService.GetEventCategoryIdentifiers(part.Categories.Select(x => x.Id));
            if (part.Categories != null)
                return part.Categories.Any() && categoryIdentifiers
                    .Contains(id);
            return false;
        }

    }
}