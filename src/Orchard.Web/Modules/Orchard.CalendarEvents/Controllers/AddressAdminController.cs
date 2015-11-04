using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.CalendarEvents.Extensions;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Orchard.CalendarEvents.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class AddressAdminController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly ITransactionManager _transactionManager;
        private readonly IAddressService _addressService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { get; set; }

        public AddressAdminController(IOrchardServices services,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IAddressService addressService)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
            _siteService = siteService;
            _transactionManager = transactionManager;
            Shape = shapeFactory;
            _addressService = addressService;
        }
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Not allowed to create event categories")))
                return new HttpUnauthorizedResult();

            var part = Services.ContentManager.New<AddressPart>("Address");
            if (part == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(part);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, T("Couldn't create address")))
                return new HttpUnauthorizedResult();

            var item = Services.ContentManager.New<AddressPart>("Address");

            _contentManager.Create(item, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(item, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(item.ContentItem);
            return Redirect(Url.AddressesForAdmin());
        }

        public ActionResult List()
        {
            IList<AddressPart> list = _addressService.GetAddressParts().ToList();

            var model = new AddressAdminListViewModel { Items = list };

            return View(model);
        }

        public ActionResult Edit(string id)
        {
            var item = _addressService.GetByIdentity(id);

            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, item, T("Not allowed to edit addresses")))
                return new HttpUnauthorizedResult();

            if (item == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(item);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(string id)
        {
            var item = _addressService.GetByIdentity(id);

            if (!Services.Authorizer.Authorize(Permissions.ManageEvents, item, T("Not allowed to edit addresses")))
                return new HttpUnauthorizedResult();

            if (item == null)
                return HttpNotFound();

            _contentManager.UpdateEditor(item, this);

            return Redirect(Url.AddressesForAdmin());
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageAddresses, T("Not allowed to delete address")))
                return new HttpUnauthorizedResult();

            var item = _addressService.GetAddressContentItem(id);

            if (item == null)
                return HttpNotFound();

            _addressService.Delete(item);

            Services.Notifier.Information(T("Address was successfully deleted"));
            return Redirect(Url.AddressesForAdmin());
        }

        public ActionResult Item(string id, PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var part = _addressService.GetByIdentity(id);

            if (part == null)
                return HttpNotFound();

            var category = Services.ContentManager.BuildDisplay(part, "DetailAdmin");

            return View(category);
        }

        public ActionResult RenderEventAddressDisplay(string addressId)
        {
            //if (!string.IsNullOrWhiteSpace(addressId))
            //{
            //    var model = _addressService.GetEventAddressByIdentity(addressId);

            //    return PartialView(model);
            //}
            return null;
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