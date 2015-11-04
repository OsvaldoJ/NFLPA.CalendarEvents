using JetBrains.Annotations;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Orchard.CalendarEvents.Drivers
{
    [UsedImplicitly]
    public class EventCategoryPartDriver : ContentPartDriver<CategoryPart>
    {
        private const string TemplateName = "Parts/EventCategory";

        protected override string Prefix
        {
            get { return "EventCategory"; }
        }

        public EventCategoryPartDriver(
            IEventService eventService
            )
        {
        }

        protected override DriverResult Display(CategoryPart part, string displayType, dynamic shapeHelper)
        {
            if (displayType == "SummaryAdmin")
            {
                return ContentShape("Parts_EventCategory_SummaryAdmin",
                    () => shapeHelper.Parts_EventCategory_SummaryAdmin(
                    ContentPart: part
                ));
            }

            return ContentShape("Parts_EventCategory_Detail",
                            () => shapeHelper.Parts_EventCategory_Detail(
                                ContentPart: part
                                ));
        }

        protected override DriverResult Editor(CategoryPart part, dynamic shapeHelper)
        {
            var model = part;

            return ContentShape("Parts_EventCategory_Edit",
                                () => shapeHelper
                                    .EditorTemplate(
                                        TemplateName: TemplateName,
                                        Model: model,
                                        Prefix: Prefix
                                    ));

        }

        protected override DriverResult Editor(CategoryPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }


        protected override void Importing(CategoryPart part, ImportContentContext context)
        {
            var description = context.Attribute(part.PartDefinition.Name, "Description");
            if (description != null)
            {
                part.Description = description;
            }

            var categoryName = context.Attribute(part.PartDefinition.Name, "CategoryName");
            if (categoryName != null)
            {
                part.CategoryName = categoryName;
            }

        }

        protected override void Exporting(CategoryPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CategoryName", part.CategoryName);
        }
    }
}