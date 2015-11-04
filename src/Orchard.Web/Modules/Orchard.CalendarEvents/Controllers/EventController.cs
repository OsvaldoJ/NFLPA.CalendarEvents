using System.Web.Mvc;
using Orchard.CalendarEvents.Formatters;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Navigation;

namespace Orchard.CalendarEvents.Controllers
{
    [ValidateInput(false), Themed]
    public class EventController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly IEventService _eventService;

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public EventController(IOrchardServices services, 
            IEventService eventService)
        {
            T = NullLocalizer.Instance;
            _eventService = eventService;
            _services = services;
        }

        public IUser CurrentUser()
        {
            return _services.WorkContext.CurrentUser;
        }

        public ActionResult Item(string eventId, PagerParameters pagerParameters)
        {
            //Display by date range, not pager
            
            var eventPart = _eventService.GetEventPart(eventId, VersionOptions.Published).As<EventPart>();

            if (eventPart == null || !_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, eventPart.ContentItem))
                return HttpNotFound();
            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, eventPart, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic eventView = _services.ContentManager.BuildDisplay(eventPart);
            
            return new ShapeResult(this, eventView);
        }

        public ContentResult Subscribe(string eventId)
        {
            var eventPart = _eventService.GetEventPart(eventId, VersionOptions.Latest);
            var ics = _eventService.GetEventIcs(eventPart);
            var downloadFileName = ics.Title.Replace(' ', '_') + ".ics";
            const string contentType = "text/iCal";
            var response = new iCalFormatResult(contentType, ics);
            //response.FileDownloadName = downloadFileName;
            return response;
        }
    }
}