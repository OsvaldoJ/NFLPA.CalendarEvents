using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Orchard.CalendarEvents.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Orchard.CalendarEvents.Drivers
{
    [UsedImplicitly]
    public class AddressPartDriver : ContentPartDriver<AddressPart>
    {
        private const string TemplateName = "Parts/Address";

        protected override string Prefix
        {
            get { return "Address"; }
        }

        protected override DriverResult Display(AddressPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Address",
                            () => shapeHelper.Parts_Address(
                                ContentPart: part,
                                LocationName: part.LocationName,
                                MapEmbedCode: part.MapEmbedCode,
                                StreetAddress1: part.StreetAddress1,
                                StreetAddress2: part.StreetAddress2,
                                StreetAddress3: part.StreetAddress3,
                                City: part.City,
                                Zip: part.Zip,
                                State: part.State,
                                Country: part.Country));
        }

        protected override DriverResult Editor(AddressPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Address_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: part,
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(AddressPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            if (part.ContentItem.Id != 0)
            {
                
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(AddressPart part, ImportContentContext context)
        {
            foreach (var prop in part.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name))
            {
                var data = context.Attribute(part.PartDefinition.Name, prop);
                if (data == null) continue;
                switch (prop)
                {
                    case "LocationName":
                        part.LocationName = data;
                        break;
                    case "StreetAddress1":
                        part.StreetAddress1 = data;
                        break;
                    case "StreetAddress2":
                        part.StreetAddress2 = data;
                        break;
                    case "StreetAddress3":
                        part.StreetAddress3 = data;
                        break;
                    case "City":
                        part.City = data;
                        break;
                    case "State":
                        part.State = data;
                        break;
                    case "Zip":
                        part.Zip = data;
                        break;
                    case "Country":
                        part.Country = data;
                        break;
                    case "MapEmbedCode":
                        part.MapEmbedCode = data;
                        break;
                }
            }


        }

        protected override void Exporting(AddressPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("LocationName", part.LocationName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StreetAddress1", part.StreetAddress1);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StreetAddress2", part.StreetAddress2);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StreetAddress3", part.StreetAddress3);
            context.Element(part.PartDefinition.Name).SetAttributeValue("City", part.City);
            context.Element(part.PartDefinition.Name).SetAttributeValue("State", part.State);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Zip", part.Zip);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Country", part.Country);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapEmbedCode", part.MapEmbedCode);


        }
    }
}