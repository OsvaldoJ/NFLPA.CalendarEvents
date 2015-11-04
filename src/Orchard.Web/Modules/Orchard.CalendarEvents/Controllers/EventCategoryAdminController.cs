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
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Orchard.CalendarEvents.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class EventCategoryAdminController : Controller, IUpdateModel
    {
        #region Constructors
        private readonly IEventService _eventService;
        private readonly ICategoryService _eventCategoryService;
        private readonly ITransactionManager _transactionManager;
        private readonly IContentManager _contentManager;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { [UsedImplicitly] get; set; }

        public EventCategoryAdminController(IOrchardServices services,
            IEventService eventService,
            ICategoryService eventCategoryService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            IShapeFactory shapeFactory)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _eventService = eventService;
            _eventCategoryService = eventCategoryService;
            Shape = shapeFactory;
        }

        #endregion


        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, T("Not allowed to create event categories")))
                return new HttpUnauthorizedResult();

            var part = Services.ContentManager.New<CategoryPart>("EventCategory");
            if (part == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(part);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, T("Couldn't create event category")))
                return new HttpUnauthorizedResult();

            var item = Services.ContentManager.New<CategoryPart>("EventCategory");

            Services.ContentManager.Create(item, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(item, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }

            Services.ContentManager.Publish(item.ContentItem);
            return Redirect(Url.EventCategoriesForAdmin());
        }

        public ActionResult List()
        {
            IList<CategoryPart> categories = _eventCategoryService.GetEventCategories().ToList();
            IList<EventCategoryViewModel> viewModelList = categories.Select(x => new EventCategoryViewModel
            {
                CategoryPart = x,
                Events = _eventService.GetEventsForCategoryIds(new[] { x.Identifier })
            }).ToList();
            var model = new EventCategoriesListViewModel
            {
                CategoryEntries = categories,
                Categories = viewModelList
            };

            return View(model);
        }

        public JsonResult GetEventCategoriesJson()
        {
            var categories = _eventCategoryService.GetEventCategories().Select(x => new
            {
                x.Identifier, x.CategoryName
            });
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventsForCategory(string categoryId)
        {
            var data = _eventService.GetEventsForCategoryIds(new[] { categoryId })
                .Select(x => new
                {
                    x.Id,
                    x.Identifier
                });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveEventsForCategoryApi(string categoryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.EventSuperAdmin, T("Couldn't bulk delete events, permission denied.")))
                return Json(null, JsonRequestBehavior.AllowGet);

            var events =
                _eventService.GetEventsForCategoryIds(new [] { categoryId })
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

            return GetEventsForCategory(categoryId);
        }

        public ActionResult Edit(string categoryId)
        {
            var item = _eventCategoryService.Get(categoryId);

            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, item, T("Not allowed to edit event categories")))
                return new HttpUnauthorizedResult();

            if (item == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(item);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(string categoryId)
        {
            var item = _eventCategoryService.Get(categoryId);

            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, item, T("Not allowed to edit event categories")))
                return new HttpUnauthorizedResult();

            if (item == null)
                return HttpNotFound();

            _contentManager.UpdateEditor(item, this);

            return Redirect(Url.EventCategoriesForAdmin());
        }

        [HttpPost]
        public ActionResult Remove(string categoryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, T("Couldn't delete event category")))
                return new HttpUnauthorizedResult();

            var item = _eventCategoryService.Get(categoryId);

            if (item == null)
                return HttpNotFound();

            //Todo: delete events that only have this category assigned to them


            _eventCategoryService.Delete(item);

            Services.Notifier.Information(T("Event Category was successfully deleted"));
            return Redirect(Url.EventCategoriesForAdmin());
        }

        [HttpPost]
        public ActionResult RemoveEventsForCategory(string categoryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEventTypes, T("Couldn't delete event category")))
                return new HttpUnauthorizedResult();

            var events = _eventService.GetEventsForCategoryIds(new [] {categoryId});

            foreach (var eventPart in events)
            {
                var item = eventPart.ContentItem;
                _contentManager.Remove(item);
            }

            Services.Notifier.Information(T("Event Category was successfully deleted"));
            return Redirect(Url.EventCategoriesForAdmin());
        }

        public ActionResult Item(string categoryId, PagerParameters pagerParameters)
        {
            //var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var part = _eventCategoryService.Get(categoryId).As<CategoryPart>();

            if (part == null)
                return HttpNotFound();

            var category = Services.ContentManager.BuildDisplay(part, "DetailAdmin");

            return View(category);
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