using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using DDay.iCal;
//using DDay.iCal.Serialization;
using Orchard.CalendarEvents.Formatters;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.Autoroute.Models;

namespace Orchard.CalendarEvents.Controllers
{
    [ValidateInput(false), Themed]
    public class CalendarController : Controller
    {
        private readonly ICalendarService _calendarService;
        private readonly IEventService _eventService;

        public IOrchardServices Services { get; set; }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public CalendarController(IOrchardServices services, 
            ICalendarService calendarService, 
            IEventService eventService, 
            IShapeFactory shapeFactory)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _calendarService = calendarService;
            _eventService = eventService;
            Shape = shapeFactory;
        }

        public ActionResult List()
        {
            var calendars = _calendarService.Get()
                .Where(b => Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, b))
                .Select(b => Services.ContentManager.BuildDisplay(b, "Summary"));

            var list = Shape.List();
            list.AddRange(calendars);

            var viewModel = Shape.ViewModel()
                .ContentItems(list);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EventsList(string calendarId)
        {
            var calendar = _calendarService.GetCalendar(calendarId, VersionOptions.Published);
            if (calendar != null && Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, calendar.ContentItem))
            {
                List<EventPart> events = _eventService.GetEventsForCalendar(calendar,VersionOptions.Published)
                    .Where(b => Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, b))
                    .ToList();
                return Json(events);
            }
            return HttpNotFound();
        }

        public ActionResult Item(string calendarId, PagerParameters pagerParameters)
        {
            //Display by date range, not pager
            
            var calendarPart = _calendarService.Get(calendarId, VersionOptions.Published).As<CalendarPart>();
            if (calendarPart == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, calendarPart, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();
            }

            //_feedManager.Register(calendarPart, Services.ContentManager.GetItemMetadata(calendarPart).DisplayText);

            dynamic calendar = Services.ContentManager.BuildDisplay(calendarPart);
            
            return new ShapeResult(this, calendar);
        }

        public JsonResult GetEvents(string calendarId, string start, string end)
        {
            var startdate = string.IsNullOrWhiteSpace(start) ? 
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : 
                DateTime.Parse(start);
            var endDate = string.IsNullOrWhiteSpace(end) ?
                new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1) :
                DateTime.Parse(end);

            var baseUrl = Services.WorkContext.CurrentSite.BaseUrl; 

            var events = _eventService.GetEventsForCalendarByDate(
                _calendarService.GetCalendar(calendarId, VersionOptions.Published),
                startdate, 
                endDate,
                VersionOptions.Published)
                

                .Select(x=>new
                {
                    title = x.Title,
                    start = x.StartDate,
                    end = x.EndDate,
                    url = string.IsNullOrWhiteSpace(x.Url) ? 
                            x.As<AutoroutePart>() != null ?
                               baseUrl + "/" + x.As<AutoroutePart>().Path : x.Url 
                            : x.Url,
                    id = x.Identifier,
                    allDay = x.AllDayEvent,
                    location = x.AddressLocation
                });

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchedulerEvents(string calendarId, string filteredCategories)
        {
            var events = _eventService.SchedulerEventsForCalendarIdentity(calendarId, filteredCategories);
            return Json(events,JsonRequestBehavior.AllowGet);
        }

        public ContentResult Subscribe(string calendarId)
        {
            var ics = _calendarService.CreateCalendarViewModel(calendarId);
            //var downloadFileName = ics.Title.Replace(' ', '_') + ".ics";
            const string contentType = "text/calendar";
            var response = new iCalFormatResult(contentType, ics);
            //response.FileDownloadName = downloadFileName;
            return response;
        }

        public ContentResult SubscribeIcs(string calendarId, string fileName)
        {
            var ics = _calendarService.CreateCalendarViewModel(calendarId);
            //var downloadFileName = ics.Title.Replace(' ', '_') + ".ics";
            const string contentType = "text/calendar";
            var response = new iCalFormatResult(contentType, ics);
            var stringContent = response.getResponseAsString();
            return Content(stringContent, contentType, System.Text.Encoding.UTF8);
            //response.FileDownloadName = downloadFileName;
            //return response;
        }
    }
    
}