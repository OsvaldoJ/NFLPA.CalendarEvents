using System.Collections.Generic;
using System.Linq;
using Orchard.CalendarEvents.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace Orchard.CalendarEvents.Services
{

    public interface IAddressService : IDependency
    {
        void UpdateEventForContentItem(ContentItem item, AddressPart model);
        ContentItem GetAddressContentItem(string identifier);
        AddressPart GetByLocationName(string locationName);
        AddressPart GetByIdentity(string identifier);
        AddressPart GetById(int id);
        IEnumerable<AddressPart> GetAddressParts(VersionOptions version = null);
        //IEnumerable<EventAddress> GetEventAddresses(VersionOptions version = null);
        //EventAddress GetEventAddressByLocationName(string locationName);
        //EventAddress GetEventAddressByIdentity(string identifier);
        //EventAddress GetEventAddress(AddressPart model);
        //string CreateAddress(EventAddress address);
        void Delete(ContentItem eventItem);
    }

    public class AddressService : IAddressService
    {
        private readonly IContentManager _contentManager;
        public IOrchardServices Services { get; set; }
        public AddressService(IContentManager contentManager,IOrchardServices services)
        {
            _contentManager = contentManager;
            Services = services;
        }

        public IEnumerable<AddressPart> GetAddressParts(VersionOptions version = null)
        {
            version = version ?? VersionOptions.Latest;
            var data = _contentManager
                .Query<AddressPart>(version)
                .List<AddressPart>()
                .OrderBy(x => x.LocationName);
            return data;
        } 
        
        public AddressPart GetByIdentity(string identifier)
        {
            var itemPart = GetAddressParts()
                .SingleOrDefault(x => x.As<IdentityPart>().Identifier == identifier);
            return itemPart;
        }

        public AddressPart GetByLocationName(string locationName)
        {
            var itemPart = GetAddressParts()
                .SingleOrDefault(x => x.LocationName.ToLower() == locationName.ToLower());
            return itemPart;
        }

        /// <summary>
        /// Used only when deleting an address item from admin portal
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public ContentItem GetAddressContentItem(string identifier)
        {
            var itemPart = GetByIdentity(identifier);
            return itemPart != null ? itemPart.ContentItem : null ;
        }

        public AddressPart GetById(int id)
        {
            var itemPart = _contentManager.Get<AddressPart>(id, VersionOptions.Latest);
            return itemPart;
        }


        //public EventAddress GetEventAddressByLocationName(string locationName)
        //{
        //    var model = GetByLocationName(locationName);
        //    return GetEventAddress(model);
        //}

        //public EventAddress GetEventAddressByIdentity(string identifier)
        //{
        //    AddressPart model = GetByIdentity(identifier);
        //    var data = GetEventAddress(model);
        //    return data;
        //}

        //public EventAddress GetEventAddress(AddressPart model)
        //{
        //    var part = new EventAddress();
        //    if (model == null) return part;
        //    part.AddressId = model.Identifier;
        //    part.LocationName = model.LocationName;
        //    part.MapEmbedCode = model.MapEmbedCode;
        //    part.StreetAddress1 = model.StreetAddress1;
        //    part.StreetAddress2 = model.StreetAddress2;
        //    part.StreetAddress3 = model.StreetAddress3;
        //    part.City = model.City;
        //    part.State = model.State;
        //    part.Zip = model.Zip;
        //    part.Country = model.Country;
        //    return part;
        //}

        //public IEnumerable<EventAddress> GetEventAddresses(VersionOptions version = null)
        //{
        //    var data = GetAddressParts(version).Select(GetEventAddress);
        //    return data;
        //}

        //public void UpdateAddressPartFromEventAddress(AddressPart part, EventAddress model)
        //{
        //    if(model==null) return;
            
        //    part.LocationName = model.LocationName;
        //    part.MapEmbedCode = model.MapEmbedCode;
        //    part.StreetAddress1 = model.StreetAddress1;
        //    part.StreetAddress2 = model.StreetAddress2;
        //    part.StreetAddress3 = model.StreetAddress3;
        //    part.City = model.City;
        //    part.State = model.State;
        //    part.Zip = model.Zip;
        //    part.Country = model.Country;
        //}

        //public string CreateAddress(EventAddress address)
        //{
        //    if (!string.IsNullOrWhiteSpace(address.AddressId))
        //    {
        //        return null;
        //    }

        //    var item = Services.ContentManager.New<AddressPart>("Address");
        //    _contentManager.Create(item, VersionOptions.Draft);
        //    var model = item.ContentItem.As<AddressPart>();
        //    UpdateAddressPartFromEventAddress(model,address);
        //    _contentManager.Publish(model.ContentItem);
        //    return model.ContentItem.Get<IdentityPart>().Identifier;
        //}

        public void UpdateEventForContentItem(ContentItem item, AddressPart model)
        {
            var part = item.As<AddressPart>();
            part.LocationName = model.LocationName;
            part.MapEmbedCode = model.MapEmbedCode;
            part.StreetAddress1 = model.StreetAddress1;
            part.StreetAddress2 = model.StreetAddress2;
            part.StreetAddress3 = model.StreetAddress3;
            part.City = model.City;
            part.State = model.State;
            part.Zip = model.Zip;
            part.Country = model.Country;

        }

        public void Delete(ContentItem item)
        {
            _contentManager.Remove(item);
        }

        //private bool ValidateEventAddress(EventAddress address)
        //{
        //    return (!string.IsNullOrWhiteSpace(address.LocationName) &&
        //            !string.IsNullOrWhiteSpace(address.StreetAddress1) &&
        //            !string.IsNullOrWhiteSpace(address.City) &&
        //            !string.IsNullOrWhiteSpace(address.State) &&
        //            !string.IsNullOrWhiteSpace(address.Zip));
        //}
    }
}