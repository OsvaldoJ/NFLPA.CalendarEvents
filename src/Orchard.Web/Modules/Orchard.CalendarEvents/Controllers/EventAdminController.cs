using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Orchard.CalendarEvents.Extensions;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Orchard.CalendarEvents.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class EventAdminController : Controller, IUpdateModel
    {
        private readonly ICalendarService _calendarService;
        private readonly IEventService _eventService;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IAutorouteService _autorouteService;

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { [UsedImplicitly] get; set; }
        public EventAdminController(IOrchardServices services,
            ICalendarService calendarService,
            IEventService eventService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            IShapeFactory shapeFactory,
            IAutorouteService ar)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _contentManager = contentManager;

            _calendarService = calendarService; //new CalendarService(_contentManager);
            _eventService = eventService;//new EventService(_contentManager);
            _transactionManager = transactionManager;
            _autorouteService = ar;
            Shape = shapeFactory;
        }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Not allowed to create events")))
                return new HttpUnauthorizedResult();

            var eventpart = Services.ContentManager.New<EventPart>("Event");
            if (eventpart == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(eventpart);
            return View(model);
        }

        public ActionResult List(string calendarId)
        {
            //using angular to populate list
            var model = new EventAdminListViewModel
            {
                CalendarId = calendarId,
                IsSuperAdmin = Services.Authorizer.Authorize(Permissions.EventSuperAdmin)
            };

            if (!string.IsNullOrWhiteSpace(calendarId))
            {
                model.Calendar = _calendarService.GetCalendar(calendarId, VersionOptions.Latest);
            }

            return View(model);
        }

        public ActionResult Edit(string eventId)
        {
            var item = _eventService.GetEventPart(eventId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, item, T("Not allowed to edit events")))
                return new HttpUnauthorizedResult();

            if (item == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(item);
            return View(model);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't create event")))
                return new HttpUnauthorizedResult();

            var item = Services.ContentManager.New<EventPart>("Event");

            _contentManager.Create(item, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(item, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }
            _contentManager.Publish(item.ContentItem);
            Services.Notifier.Information(T("Event was successfully created"));
            return Redirect(Url.EventEdit(item.Identifier));
            //return Redirect(Url.EventsForAdmin());
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(string eventId)
        {
            var item = _eventService.GetEventPart(eventId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, item, T("Not allowed to edit events")))
                return new HttpUnauthorizedResult();

            var model = _contentManager.UpdateEditor(item, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(item.ContentItem);

            //Update alias for autoroute
            var autoroutePart = item.As<AutoroutePart>();
            if (autoroutePart != null)
            {
                _autorouteService.PublishAlias(autoroutePart);
            }

            Services.Notifier.Information(T("Event was successfully edited"));
            return View(model);
            //return Redirect(Url.EventsForAdmin());
        }

        [HttpPost]
        public ActionResult Remove(string eventId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't delete event")))
                return new HttpUnauthorizedResult();

            var item = _eventService.GetEventPart(eventId, VersionOptions.Latest);

            if (item == null)
                return HttpNotFound();

            _eventService.Delete(item.ContentItem);

            Services.Notifier.Information(T("Event was successfully deleted"));
            return Redirect(Url.EventsForAdmin());
        }

        public ActionResult Item(string eventId, PagerParameters pagerParameters)
        {
            var eventPart = _eventService.GetEventPart(eventId, VersionOptions.Latest);

            if (eventPart == null)
                return HttpNotFound();

            var eventItem = Services.ContentManager.BuildDisplay(eventPart, "Detail");
            
            return View(eventItem);
        }

        [HttpPost, ActionName("CreateEventFromScheduler")]
        public JsonResult CreateEventFromSchedulerPOST(string models)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't create event")))
                return Json(null, JsonRequestBehavior.AllowGet);

            var modelFromJson = JsonConvert.DeserializeObject<List<SchedulerEventViewModel>>(models);
            if (!modelFromJson.Any())
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var returnModel = new List<SchedulerEventViewModel>();

            foreach (var schedulerEventViewModel in modelFromJson)
            {
                //require title
                if (string.IsNullOrWhiteSpace(schedulerEventViewModel.Title))
                {
                    continue;
                }
                var checkDupes = schedulerEventViewModel.CheckDuplicates ?? false;
                if (checkDupes)
                {
                    EventPart eventItem;
                    //check for duplicates
                    //checks for google importUid
                    if (!string.IsNullOrWhiteSpace(schedulerEventViewModel.ImportUid))
                    {
                        //this slows the import process down, but may be a necessary evil
                        eventItem = _contentManager.Query<EventPart>(VersionOptions.Latest)
                            .List().SingleOrDefault(x => x.ImportUid == schedulerEventViewModel.ImportUid);
                    }
                    else
                    {
                        //check for duplicates by title and start / end dates
                        eventItem = _contentManager.Query<EventPart>(VersionOptions.Latest)
                            .List().SingleOrDefault(x => string.Equals(x.Title, schedulerEventViewModel.Title,StringComparison.OrdinalIgnoreCase) 
                                && string.Equals(Convert.ToDateTime(x.StartDate).ToShortDateString(), schedulerEventViewModel.Start.ToShortDateString(), StringComparison.OrdinalIgnoreCase));
                    } 

                    if (eventItem != null)
                    {
                        //update already existing event
                        _calendarService.UpdateCalendarForSchedulerEventViewModel(eventItem.ContentItem,
                            schedulerEventViewModel);

                        if (!ModelState.IsValid)
                        {
                            _transactionManager.Cancel();
                            //return Json(null, JsonRequestBehavior.AllowGet);
                            continue;
                        }

                        _contentManager.Publish(eventItem.ContentItem);

                        returnModel.Add(_eventService.SchedulerEventViewModelFromEvent(eventItem));
                        //return Json(returnModel, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var item = Services.ContentManager.New<EventPart>("Event");

                    _contentManager.Create(item, VersionOptions.Draft);

                    var model = schedulerEventViewModel;

                    _calendarService.UpdateCalendarForSchedulerEventViewModel(item.ContentItem, model);

                    item.As<ICommonPart>().Owner = Services.WorkContext.CurrentUser;

                    _contentManager.Publish(item.ContentItem);

                    var updatedModel = _eventService.SchedulerEventViewModelFromEvent(item);
                    returnModel.Add(updatedModel);
                }

            }


            return Json(returnModel, JsonRequestBehavior.DenyGet);
        }

        public JsonResult CreateEventFromScheduler(string models)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't create event")))
                return Json(null,JsonRequestBehavior.AllowGet);

            var modelFromJson = JsonConvert.DeserializeObject<List<SchedulerEventViewModel>>(models);
            if (!modelFromJson.Any())
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var returnModel = new List<SchedulerEventViewModel>();

            foreach (var schedulerEventViewModel in modelFromJson)
            {
                var checkDupes = schedulerEventViewModel.CheckDuplicates ?? false;
                if (checkDupes && !string.IsNullOrWhiteSpace(schedulerEventViewModel.ImportUid))
                {
                    //check for duplicates
                    //this slows the import process down, but may be a necessary evil
                    var eventItem = _contentManager.Query<EventPart>(VersionOptions.Latest)
                        .List().SingleOrDefault(x => x.ImportUid == schedulerEventViewModel.ImportUid);
                    if (eventItem != null)
                    {
                        //update already existing event
                        _calendarService.UpdateCalendarForSchedulerEventViewModel(eventItem.ContentItem,
                            schedulerEventViewModel);

                        if (!ModelState.IsValid)
                        {
                            _transactionManager.Cancel();
                            return Json(null, JsonRequestBehavior.AllowGet);
                        }

                        _contentManager.Publish(eventItem.ContentItem);

                        returnModel.Add(_eventService.SchedulerEventViewModelFromEvent(eventItem));
                    }
                }
                else
                {
                    var item = Services.ContentManager.New<EventPart>("Event");

                    _contentManager.Create(item, VersionOptions.Draft);

                    var model = schedulerEventViewModel;

                    _calendarService.UpdateCalendarForSchedulerEventViewModel(item.ContentItem, model);

                    item.As<ICommonPart>().Owner = Services.WorkContext.CurrentUser;

                    _contentManager.Publish(item.ContentItem);

                    var updatedModel = _eventService.SchedulerEventViewModelFromEvent(item);
                    returnModel.Add(updatedModel);
                }

            }


            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateEventFromScheduler(string models)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't update event")))
                return Json(null, JsonRequestBehavior.AllowGet);

            var modelFromJson = JsonConvert.DeserializeObject<List<SchedulerEventViewModel>>(models);
            if (!modelFromJson.Any())
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var returnModel = new List<SchedulerEventViewModel>();

            foreach (var schedulerEventViewModel in modelFromJson)
            {

                var model = schedulerEventViewModel;

                var eventIdentifier = model.Identifier;

                var item = _eventService.GetEventPart(eventIdentifier, VersionOptions.Latest);

                _calendarService.UpdateCalendarForSchedulerEventViewModel(item.ContentItem, model);

                if (!ModelState.IsValid)
                {
                    _transactionManager.Cancel();
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                _contentManager.Publish(item.ContentItem);

                var updatedModel = _eventService.SchedulerEventViewModelFromEvent(item.As<EventPart>());
            }


            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEventFromScheduler(string models)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't delete event")))
                return Json(null, JsonRequestBehavior.AllowGet);

            var modelFromJson = JsonConvert.DeserializeObject<List<SchedulerEventViewModel>>(models);
            if (!modelFromJson.Any())
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var returnModel = new List<SchedulerEventViewModel>();
            foreach (var model in modelFromJson)
            {
                var eventIdentifier = model.Identifier;
                var item = _eventService.GetEventPart(eventIdentifier, VersionOptions.Latest);
                returnModel.Add(_eventService.SchedulerEventViewModelFromEvent(item));
                if (item.ContentItem != null)
                {
                    _contentManager.Remove(item.ContentItem);
                }
            }
            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs("GET")]
        public JsonResult GetEventByIdentifier(string eventIdentifier)
        {
            var viewModel = _eventService.SchedulerEventViewModelFromEvent(_eventService.GetEventPart(eventIdentifier, VersionOptions.Latest));
            return Json(viewModel,
                JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET")]
        public JsonResult SearchCalendar(string calendarId, string title, string startDate, string endDate, int? currentPage = null,
            int? resultsPerPage = null, string eventCategoryIdsCsv = null)
        {
            var data = Json(_eventService.SearchInCalendar(calendarId, title, startDate, endDate, currentPage, resultsPerPage, null, eventCategoryIdsCsv),
                JsonRequestBehavior.AllowGet);
            return data;
        }

        [AcceptVerbs("GET")]
        public JsonResult Search(string title, string startDate, string endDate, int? currentPage = null,
            int? resultsPerPage = null, string eventCategoryIdsCsv = null, string calendarId = null)
        {
            if (!string.IsNullOrWhiteSpace(calendarId))
            {
                return SearchCalendar(calendarId, title, startDate, endDate, currentPage, resultsPerPage,
                    eventCategoryIdsCsv);
            }

            var data = Json(_eventService.Search(title, startDate, endDate, currentPage, resultsPerPage, null, eventCategoryIdsCsv),
                JsonRequestBehavior.AllowGet);
            return data;
        }

        public JsonResult GetAllEventIds()
        {

            var events =
                _eventService
                .GetAllEvents(VersionOptions.Latest)
                .Select(x=>x.Id)
                .ToList();

            var data = Json(events,
                JsonRequestBehavior.AllowGet);
            return data; ;
        }
        public JsonResult RemoveAllEventsApi()
        {
            if (!Services.Authorizer.Authorize(Permissions.EventSuperAdmin, T("Couldn't bulk delete events, permission denied.")))
                return Json(null, JsonRequestBehavior.AllowGet);

            var events =
                _eventService
                .GetAllEvents(VersionOptions.Latest)
                //.Select(x=>x.Id)
                .ToList();

            const int batchSize = 50;
            var count = 0;

            foreach (var eventPart in events)
            {
                var item = eventPart.ContentItem;
                _contentManager.Remove(item);

                if (++count % batchSize == 0)
                {
                    _transactionManager.RequireNew();
                }
            }

            var data = Json(_eventService.Search("", "", ""),
                JsonRequestBehavior.AllowGet);
            return data;;
        }

        public JsonResult RemoveEventsById(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var eventItem = _contentManager.Get(Int32.Parse(id));

                if (eventItem != null)
                {
                    _contentManager.Remove(eventItem);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        
        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

    }
}