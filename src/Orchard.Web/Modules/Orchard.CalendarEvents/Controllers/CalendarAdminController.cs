using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;
using Orchard.CalendarEvents.Extensions;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Orchard.CalendarEvents.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class CalendarAdminController : Controller, IUpdateModel
    {
        private readonly ICalendarService _calendarService;
        private readonly ICategoryService _eventCategoryService;
        private readonly ITransactionManager _transactionManager;
        private readonly IContentManager _contentManager;

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { [UsedImplicitly] get; set; }

        public CalendarAdminController(IOrchardServices services, 
            ICalendarService calendarService,
            ICategoryService eventCategoryService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            IShapeFactory shapeFactory)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
            _transactionManager = transactionManager;

            _calendarService = calendarService;
            _eventCategoryService = eventCategoryService;
            Shape = shapeFactory;
        }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, T("Not allowed to create calendars")))
                return new HttpUnauthorizedResult();

            var calendar = Services.ContentManager.New<CalendarPart>("Calendar");
            if (calendar == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(calendar);

            return View(model);
        }

        public ActionResult Edit(string calendarId)
        {
            var calendar = _calendarService.Get(calendarId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, calendar, T("Not allowed to edit calendars")))
                return new HttpUnauthorizedResult();

            if (calendar == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(calendar);
            return View(model);
        }


        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, T("Couldn't create calendar")))
                return new HttpUnauthorizedResult();

            var item = Services.ContentManager.New<CalendarPart>("Calendar");

            _contentManager.Create(item, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(item, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }
            _contentManager.Publish(item.ContentItem);
            return Redirect(Url.CalendarsForAdmin());
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(string calendarId)
        {
            var item = _calendarService.Get(calendarId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, item, T("Not allowed to edit calendars")))
                return new HttpUnauthorizedResult();

            var model = _contentManager.UpdateEditor(item, this);
            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }
            return View(model);//Redirect(Url.CalendarForAdmin(item.As<CalendarPart>()));
        }

        [HttpPost]
        public ActionResult Remove(string calendarId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, T("Couldn't delete calendar")))
                return new HttpUnauthorizedResult();

            var calendar = _calendarService.Get(calendarId, VersionOptions.Latest);

            if (calendar == null)
                return HttpNotFound();

            _calendarService.Delete(calendar);

            Services.Notifier.Information(T("Calendar was successfully deleted"));
            return Redirect(Url.CalendarsForAdmin());
        }

        [HttpPost]
        public ActionResult RemoveById(int calendarId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageCalendars, T("Couldn't delete calendar")))
                return new HttpUnauthorizedResult();

            var calendar = _calendarService.Get(calendarId.ToString(), VersionOptions.Latest);

            if (calendar == null)
                return HttpNotFound();

            _calendarService.Delete(calendar);

            Services.Notifier.Information(T("Calendar was successfully deleted"));
            return Redirect(Url.CalendarsForAdmin());
        }

        public ActionResult List()
        {
            IList<CalendarPart> calendars = _calendarService.Get().ToList();

            var model = new CalendarAdminListViewModel
            {
                Calendars = calendars,
                CategoriesList = _eventCategoryService.GetEventCategories().ToList()
            };
            return View(model);
        }

        public ActionResult Item(string calendarId)
        {
            //return Edit(calendarId);
            
            var calendarPart = _calendarService.Get(calendarId, VersionOptions.Latest).As<CalendarPart>();

            if (calendarPart == null)
                return HttpNotFound();

            var calendar = Services.ContentManager.BuildDisplay(calendarPart, "DetailAdmin"); //"DetailAdmin" for Kendo Scheduler

            return View(calendar);
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