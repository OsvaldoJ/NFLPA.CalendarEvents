using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.CalendarEvents.Extensions;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.Alias;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Settings;

namespace Orchard.CalendarEvents.Services
{
    public interface IEventService : IDependency
    {
        IEnumerable<EventPart> GetEventsForCalendar(CalendarPart calendarItem, VersionOptions versionOptions);
        IEnumerable<EventPart> GetAllEvents(VersionOptions options);
        EventPart GetParentEvent(EventPart subEvent);
        IEnumerable<EventPart> GetSubEvents(EventPart parentEvent);
        IEnumerable<EventPart> GetRelatedEvents(EventPart part);
        EventPart GetEventPart(string identifier, VersionOptions versionOptions = null);
        void Delete(ContentItem eventItem);
        void UpdateContentItemFromModel(ContentItem item, EditEventViewModel model);
        EventIcs GetEventIcs(EventPart x);
        IEnumerable<EventIcs> GetEventIcsList(IEnumerable<EventPart> events);
        IEnumerable<SchedulerEventViewModel> SchedulerEventsForCalendarIdentity(string calendarId, string filteredEventCategoriesCsv);
        SchedulerEventViewModel SchedulerEventViewModelFromEvent(EventPart x);
        IEnumerable<EventPart> GetEventsForCalendarByDate(CalendarPart calendar, DateTime? startDate, DateTime? endDate, VersionOptions versionOptions);
        IEnumerable<EventPart> GetAllUpcomingEvents(VersionOptions versionOptions = null);
        IEnumerable<EventPart> GetUpcomingEventsForCalendar(CalendarPart calendar, VersionOptions versionOptions, int count);
        EventSearchViewModel SearchInCalendar(string calendarIdentifier, string title, string startDate, string endDate, int? currentPage = null, 
            int? resultsPerPage = null, VersionOptions options = null, string overrideCategoryIdsCsv = null);
        EventSearchViewModel Search(string title, string startDate, string endDate, int? currentPage = null,
            int? resultsPerPage = null, VersionOptions options = null, string categoryIdsCsv = null);
        IEnumerable<EventPart> GetEventsForIds(string[] ids);
        IEnumerable<EventPart> GetEventsForCategoryIds(string[] ids, DateTime? startDate = null);
    }

    public class EventService : IEventService
    {
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IAliasService _aliasService;
        private readonly ISiteService _siteService;
        private readonly ICategoryService _categoryService;
        private readonly ITransactionManager _transactionManager;

        public EventService(IContentManager contentManager,
            IOrchardServices services,
            IAliasService aliasService,
            ISiteService site, 
            ICategoryService categoryService, 
            ITransactionManager transactionManager)
        {
            _contentManager = contentManager;
            _services = services;
            _aliasService = aliasService;
            _siteService = site;
            _categoryService = categoryService;
            _transactionManager = transactionManager;
        }

        public IEnumerable<EventPart> GetSubEvents(EventPart parentEvent)
        {
            var allEvents = GetAllEvents(VersionOptions.Latest);
            var subevents = allEvents.Where(x => x.ParentEventIdentifier == parentEvent.Identifier);
            return subevents.FilterEventParts(_services.WorkContext.CurrentUser);
        }
        public EventPart GetParentEvent(EventPart subEvent)
        {
            var allEvents = GetAllEvents(VersionOptions.Latest);
            var parentEvent = allEvents.FilterEventParts(_services.WorkContext.CurrentUser).SingleOrDefault(x => x.Identifier == subEvent.ParentEventIdentifier);
            return parentEvent;
        }

        public IEnumerable<EventPart> GetRelatedEvents(EventPart part)
        {
            List<EventPart> relatedEvents = new List<EventPart>();

            if (!string.IsNullOrWhiteSpace(part.ParentEventIdentifier))
            {
                //get parent event then get siblings 
                var parentEvent = GetParentEvent(part);
                relatedEvents.Add(parentEvent);
                //get siblings - excluding current event
                var siblings = GetSubEvents(parentEvent).Where(x => x.Identifier != part.Identifier);
                relatedEvents.AddRange(siblings);
            }
            //get children and join the two lists
            var childEvents = GetSubEvents(part);
            relatedEvents.AddRange(childEvents);

            return relatedEvents;
        }

        public IEnumerable<EventPart> GetAllEvents(VersionOptions options)
        {
            return _contentManager
                .Query<EventPart>(options ?? VersionOptions.Latest)
                .Join<IdentityPartRecord>()
                .Join<EventPartRecord>()
                .List()
                .OrderBy(x => x.StartDate);
        }

        public IEnumerable<EventPart> GetEventsForCalendar(CalendarPart calendarItem, VersionOptions versionOptions)
        {
            var session = _transactionManager.GetSession(); // _sessionLocator.For(typeof(EventPartRecord));
            var eventQuery = session.CreateQuery(EventQuery(calendarItem.ContentItem.Id));
            var ids = eventQuery.List<int>();

            var events = _contentManager.GetMany<EventPart>(ids, VersionOptions.Latest, QueryHints.Empty)
                .FilterEventParts(_services.WorkContext.CurrentUser)
                .OrderBy(x => x.StartDate);
                    
            return events;
        }

        public IEnumerable<EventPart> GetUpcomingEventsForCalendar(CalendarPart calendar,
            VersionOptions versionOptions, int count) {
            if(calendar == null) {
                return new List<EventPart>();
            }
            var session = _transactionManager.GetSession(); //_sessionLocator.For(typeof (EventPartRecord));
            var eventQuery = session.CreateQuery(EventQuery(calendar.ContentItem.Id, DateTime.Today));
                //.SetMaxResults(count); // Can't Use becuase of Permission Filtering
            var ids = eventQuery.List<int>();

            var events = _contentManager.GetMany<EventPart>(ids, VersionOptions.Latest, QueryHints.Empty)
                .FilterEventParts(_services.WorkContext.CurrentUser)
                .OrderBy(x => x.StartDate);

            return count > 0 ? events.Take(count) : events;
        }

        public IEnumerable<EventPart> GetEventsForCalendarByDate(CalendarPart calendar,
            DateTime? startDate,
            DateTime? endDate,
            VersionOptions versionOptions) {

            if (calendar == null) {
                return new List<EventPart>();
            }
            var session = _transactionManager.GetSession(); //_sessionLocator.For(typeof(EventPartRecord));
            var eventQuery = session.CreateQuery(EventQuery(calendar.ContentItem.Id, startDate, endDate));
            var ids = eventQuery.List<int>();

            var events = _contentManager.GetMany<EventPart>(ids, VersionOptions.Latest, QueryHints.Empty)
                .FilterEventParts(_services.WorkContext.CurrentUser)
                .OrderBy(x => x.StartDate);

            return events;
        }

        public void Delete(ContentItem eventItem)
        {
            _contentManager.Remove(eventItem);
        }


        public void UpdateContentItemFromModel(
            ContentItem item,
            EditEventViewModel model)
        {
            var part = item.As<EventPart>();

            if (model.ParentEventIdentifier != part.Identifier &&
                !string.IsNullOrWhiteSpace(model.ParentEventIdentifier))
                part.ParentEventIdentifier = model.ParentEventIdentifier;
            part.As<TitlePart>().Title = model.Title;
            part.Description = model.Description;
            part.AddressLocation = model.AddressLocation;
            part.TimeZone = model.TimeZone;
            part.RecurrenceId = model.RecurrenceId;
            part.RecurrenceRule = model.RecurrenceRule;
            part.RecurrenceException = model.RecurrenceException;
            part.ImportedFromGoogleCalendar = false;

            _categoryService.UpdateCategoriesForEvent(item, model.SelectedEventCategoryIds.Split(','));

            part.AllDayEvent = model.AllDayEvent;
            if (model.AllDayEvent)
            {
                model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day);
                model.EndDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day);
            }
            part.StartDate = model.StartDate;
            //part.StartDate = TimeZoneHelper.ConvertTZIDtoUTC(model.StartDate, model.TimeZone);
            part.EndDate = model.EndDate;
            //part.EndDate = TimeZoneHelper.ConvertTZIDtoUTC(model.EndDate, model.TimeZone);

            //set url if blank to autoroute alias
            part.Url = model.Url;
        }


        private string GetEventCategoryNamesCSV(EditEventViewModel model)
        {
            var selectedIds = model.SelectedEventCategoryIds.Split(',');
            var values = selectedIds.Select(
                selectedId => model.Categories.Where(x => x.Identifier == selectedId)
                    .Select(y => y.Name)
                    .FirstOrDefault()
                ).ToList();

            return string.Join(",", values);
        }

        public EventIcs GetEventIcs(EventPart x)
        {
            var data = new EventIcs
            {
                IsAllDay = x.AllDayEvent,
                Id = x.Identifier,
                Title = x.Title,
                Start = Convert.ToDateTime(x.StartDate),//TimeZoneHelper.ConvertTZIDFromUTC(x.StartDate, x.TimeZone),
                End = Convert.ToDateTime(x.EndDate),//TimeZoneHelper.ConvertTZIDFromUTC(x.EndDate, x.TimeZone),
                CreateDateTime = Convert.ToDateTime(x.CreateUtc),
                ModifiedDateTime = Convert.ToDateTime(x.ModifiedUtc),
                PublishDateTime = Convert.ToDateTime(x.PublishedUtc),
                CategoriesCsv = string.Join(",", x.Categories.Select(y => y.CategoryName)),
                LocationName = x.AddressLocation,
                TimeZone = string.IsNullOrWhiteSpace(x.TimeZone) ? "America/New_York" : x.TimeZone,
                RecurrenceId = x.RecurrenceId,
                RecurrenceRule = x.RecurrenceRule,
                RecurrenceException = x.RecurrenceException,
                Url = x.Url
            };
            if (!string.IsNullOrEmpty(x.Url))
            {
                data.Url = x.Url;
            }
            else
            {
                var autoroute = x.ContentItem.As<AutoroutePart>();
                if (autoroute != null)
                {
                    data.Url = _siteService.GetSiteSettings().BaseUrl + "/" + autoroute.Path;
                }
            }
            return data;
        }

        public IEnumerable<EventIcs> GetEventIcsList(IEnumerable<EventPart> events)
        {
            var list = events.Select(GetEventIcs);
            return list;
        }

        public IEnumerable<EventPart> GetAllUpcomingEvents(VersionOptions versionOptions = null)
        {
            var events = GetAllEvents(versionOptions).Where(x => x.StartDate >= DateTime.Today);
            return events;
        }

        public EventPart GetEventPart(string identifier, VersionOptions versionOptions = null)
        {
            var data = GetAllEvents(versionOptions).SingleOrDefault(x => x.Identifier == identifier);
            return data;
        }


        public IEnumerable<SchedulerEventViewModel> SchedulerEventsForCalendarIdentity(string calendarId, string filteredEventCategoriesCsv)
        {
            IEnumerable<EventPart> events;
            if (filteredEventCategoriesCsv != null)
            {
                // Filter by Categories
                var categories = filteredEventCategoriesCsv.Split(',');
                events = GetEventsForCategoryIds(categories)
                    .OrderByDescending(x => x.StartDate);

            }
            else
            {
                // Filter by Calendar
                var calendar = _contentManager.Query<CalendarPart>(VersionOptions.Latest)
                    .Where<IdentityPartRecord>(x => x.Identifier == calendarId)
                    .List()
                    .SingleOrDefault();

                events = GetEventsForCalendarByDate(calendar, DateTime.Today, null, VersionOptions.Latest)
                    .OrderByDescending(x => x.StartDate);
            }

            return events.Select(SchedulerEventViewModelFromEvent);
            
        }

        public SchedulerEventViewModel SchedulerEventViewModelFromEvent(EventPart x)
        {
            var categories = _categoryService.GetEventCategoriesByEvent(x.ContentItem).ToList();
            var data = new SchedulerEventViewModel
            {
                Id = x.Id,
                Identifier = x.Identifier,
                Title = x.Title,
                Description = x.Description,
                IsAllDay = x.AllDayEvent,
                Start = Convert.ToDateTime(x.StartDate),
                End = Convert.ToDateTime(x.EndDate),
                Url = x.Url,
                EventCategoryIds = _categoryService.GetEventCategoryIdentifiers(categories.Select(y => y.Id)).ToArray(),
                EventCategoryNames = categories.Select(y => y.CategoryName).ToArray(),
                Timezone = x.TimeZone,
                RecurrenceId = x.RecurrenceId,
                RecurrenceRule = x.RecurrenceRule,
                RecurrenceException = x.RecurrenceException,
                Location = x.AddressLocation,
                ImportFromGoogleCalendar = x.ImportedFromGoogleCalendar,
                ImportUid = x.ImportUid
            };
            return data;
        }

        public EventSearchViewModel SearchInCalendar(string calendarIdentifier, string title, string startDate, string endDate, int? currentPage = null,
            int? resultsPerPage = null, VersionOptions options = null, string overrideCategoryIdsCsv = null)
        {
            if (!string.IsNullOrWhiteSpace(overrideCategoryIdsCsv))
                return Search(title, startDate, endDate, currentPage, resultsPerPage, null, overrideCategoryIdsCsv);

            var calendar =
                _contentManager.Query<CalendarPart>(VersionOptions.Latest)
                    .Join<IdentityPartRecord>()
                    .Join<CalendarPartRecord>()
                    .Where<IdentityPartRecord>(x => x.Identifier == calendarIdentifier)
                    .List()
                    .FirstOrDefault();

            string identifiers = null;
            if (calendar != null)
            {
                identifiers = string.Join(",", _categoryService.GetEventCategoryIdentifiers(calendar.Categories.Select(x => x.Id)));
            }

            return Search(title, startDate, endDate, currentPage, resultsPerPage, null, identifiers);
        }

        public EventSearchViewModel Search(string title, string startDate, string endDate, int? currentPage = null,
            int? resultsPerPage = null, VersionOptions options = null, string categoryIdsCsv = null)
        {
            IEnumerable<EventPart> query;

            DateTime? nullableStartDate = null;
            
            if(!string.IsNullOrWhiteSpace(startDate)){
                nullableStartDate = Convert.ToDateTime(startDate);
            }

            if (!string.IsNullOrWhiteSpace(categoryIdsCsv))
            {
                string[] ids = categoryIdsCsv.Split(',');
                query = GetEventsForCategoryIds(ids, nullableStartDate);
            }
            else
            {
                query = _contentManager
                    .Query<EventPart>(options ?? VersionOptions.Latest)
                    .Join<IdentityPartRecord>()
                    .Join<EventPartRecord>()
                    .List<EventPart>();
                //Already filtering by startdate above, so only including it if no category ids where sent in
                if (nullableStartDate != null)
                {
                    query = query.Where(x => x.StartDate >= nullableStartDate.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(endDate))
            {
                query = query.Where(x => x.EndDate <= Convert.ToDateTime(endDate));
            }

            if (!string.IsNullOrWhiteSpace(title) && title.Length > 1)
            {
                query = query.Where(x => x.Title.ToLower().Contains(title.ToLower()))
                    .OrderBy(x => x.Title);
            }
            else
            {
                query = query.OrderBy(x => x.StartDate);
            }

            var take = resultsPerPage ?? 20;
            var skip = Convert.ToInt32(currentPage != null ? (currentPage - 1) * take : 0);

            var allEvents = query.ToList();

            IEnumerable<EventPart> events = allEvents.Skip(skip).Take(take).ToList();
            //foreach (var eventPart in events)
            //{
            //    var autoroute = eventPart.ContentItem.As<AutoroutePart>();
            //    if (string.IsNullOrWhiteSpace(eventPart.Url) && autoroute != null)
            //    {
            //        eventPart.Url = _siteService.GetSiteSettings().BaseUrl + "/" + autoroute.Path;
            //    }
            //}
            var paginated = events.Select(SchedulerEventViewModelFromEvent).ToList();

            var baseUrl = _siteService.GetSiteSettings().BaseUrl;

            foreach (var i in paginated)
            {
                if (string.IsNullOrWhiteSpace(i.Url))
                {
                    var eventPart = events.SingleOrDefault(x => x.Id == i.Id);
                    if (eventPart != null)
                    {
                        var autoroute = eventPart.ContentItem.As<AutoroutePart>();
                        if (autoroute != null)
                        {
                            i.Permalink =  baseUrl + "/" + autoroute.Path;
                            i.Url = i.Permalink;
                        }
                    }
                }
            }

            var viewModel = new EventSearchViewModel
            {
                CurrentPage = currentPage.HasValue ? currentPage.Value : 1,
                NumberofPages = allEvents.Count() / (take),
                ResultsPerPage = resultsPerPage ?? 20,
                SearchResults = paginated,
                TotalItems = allEvents.Count()
            };
            return viewModel;
        }

        public IEnumerable<EventPart> GetEventsForIds(string[] ids)
        {
            var list = GetAllEvents(VersionOptions.Latest)
                .Where(x => ids.Contains(x.Identifier));
            return list;
        }

        public IEnumerable<EventPart> GetEventsForCategoryIds(string[] ids, DateTime? startDate = null)
        {
            var session = _transactionManager.GetSession(); //_sessionLocator.For(typeof(EventPartRecord));
            var eventQuery = session.CreateQuery(EventQueryByCategoryIdentifier(ids, startDate));
            var eventIds = eventQuery.List<int>();

            var events = _contentManager.GetMany<EventPart>(eventIds, VersionOptions.Latest, QueryHints.Empty)
                .FilterEventParts(_services.WorkContext.CurrentUser)
                .OrderBy(x => x.StartDate);

            return events;
        }

        private string EventQueryByCategoryIdentifier(string[] ids, DateTime? startDate)
        {
            var wherIds = ids.Select(id => string.Format("Identity.Identifier = '{0}'", id)).ToList();

            var query = "SELECT DISTINCT Event.Id " +
                        "FROM Orchard.Core.Common.Models.IdentityPartRecord Identity, " +
                        "Orchard.CalendarEvents.Models.EventCategoryJoinRecord EventJoinRecord, " +
                        "Orchard.CalendarEvents.Models.EventPartRecord Event " +
                        "WHERE (" + string.Join(" OR ", wherIds) + ")" +
                        "AND Identity.Id = EventJoinRecord.CategoryPartRecord.Id " +
                        "AND Event.Id = EventJoinRecord.EventPartRecord.Id";
            if (startDate != null)
            {
                query = query + string.Format(" AND Event.StartDate >= '{0}'", Convert.ToDateTime(startDate).ToShortDateString());
            }
            return query;
        }

        private string EventQuery(int recordId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = "SELECT DISTINCT EventJoinRecord.EventPartRecord.Id " +
                        "FROM Orchard.CalendarEvents.Models.CalendarCategoryJoinRecord CalendarJoinRecord, " +
                        "Orchard.CalendarEvents.Models.EventCategoryJoinRecord EventJoinRecord, " +
                        "Orchard.CalendarEvents.Models.EventPartRecord Event " +
                        "WHERE CalendarJoinRecord.CalendarPartRecord.Id = " + recordId +
                        "AND (CalendarJoinRecord.CategoryPartRecord.Id = EventJoinRecord.CategoryPartRecord.Id)  " +
                        "AND Event.Id = EventJoinRecord.EventPartRecord.Id";

            if (startDate != null && endDate == null)
            {
                query = query + string.Format(" AND Event.StartDate >= '{0}'", Convert.ToDateTime(startDate).ToShortDateString());
            }
            if (startDate == null && endDate != null)
            {
                query = query + string.Format(" AND Event.EndDate <= '{0}'", Convert.ToDateTime(endDate).ToShortDateString());
            }
            if (startDate != null && endDate != null)
            {
                query = query + string.Format(" AND Event.StartDate >= '{0}'", Convert.ToDateTime(startDate).ToShortDateString());
                query = query + string.Format(" AND Event.EndDate <= '{0}'", Convert.ToDateTime(endDate).ToShortDateString());
            }

            return query;
        }
    }
}