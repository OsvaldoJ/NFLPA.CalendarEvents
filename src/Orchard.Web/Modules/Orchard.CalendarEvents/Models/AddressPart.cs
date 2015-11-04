using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace Orchard.CalendarEvents.Models
{
    public class AddressPart : ContentPart
    {
        public string Identifier
        {
            get { return this.As<IdentityPart>().Identifier; }
        }
        public string StreetAddress1
        {
            get { return this.Retrieve(x => x.StreetAddress1); }
            set { this.Store(x => x.StreetAddress1, value); }
        }
        public string StreetAddress2
        {
            get { return this.Retrieve(x => x.StreetAddress2); }
            set { this.Store(x => x.StreetAddress2, value); }
        }
        public string StreetAddress3
        {
            get { return this.Retrieve(x => x.StreetAddress3); }
            set { this.Store(x => x.StreetAddress3, value); }
        }
        public string City
        {
            get { return this.Retrieve(x => x.City); }
            set { this.Store(x => x.City, value); }
        }
        public string State
        {
            get { return this.Retrieve(x => x.State); }
            set { this.Store(x => x.State, value); }
        }
        public string Zip
        {
            get { return this.Retrieve(x => x.Zip); }
            set { this.Store(x => x.Zip, value); }
        }
        public string Country
        {
            get { return this.Retrieve(x => x.Country); }
            set { this.Store(x => x.Country, value); }
        }
        public string LocationName
        {
            get { return this.Retrieve(x => x.LocationName); }
            set { this.Store(x => x.LocationName, value); }
        }
        public string MapEmbedCode
        {
            get { return this.Retrieve(x => x.MapEmbedCode); }
            set { this.Store(x => x.MapEmbedCode, value); }
        }
    }

}