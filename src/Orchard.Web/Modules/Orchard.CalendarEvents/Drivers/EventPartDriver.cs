using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.CalendarEvents.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;


namespace Orchard.CalendarEvents.Drivers
{
    [UsedImplicitly]
    public class EventPartDriver : ContentPartDriver<EventPart>
    {
        private const string TemplateName = "Parts/Event";
        private readonly IEventService _eventService;
        private readonly ICategoryService _eventCategoryService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public EventPartDriver(
            IEventService eventService,
            ICategoryService eventCategoryService,
            IOrchardServices services
            )
        {
            T = NullLocalizer.Instance;
            _eventService = eventService;
            _eventCategoryService = eventCategoryService;
            Services = services;
        }

        protected override string Prefix
        {
            get { return "Event"; }
        }

        protected override DriverResult Display(EventPart part, string displayType, dynamic shapeHelper)
        {
            if (displayType == "SummaryAdmin")
            {
                return ContentShape("Parts_Event_SummaryAdmin", 
                    () => shapeHelper.Parts_Event_SummaryAdmin(
                    ContentPart: part
                ));
            }

            return ContentShape("Parts_Event_Detail",
                            () => shapeHelper.Parts_Event_Detail(
                                ContentPart: part
                                ));
        }

        protected override DriverResult Editor(EventPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Event_Edit",
                                () => shapeHelper
                                    .EditorTemplate(
                                        TemplateName: TemplateName,
                                        Model: CreateEditViewModel(part), 
                                        Prefix: Prefix
                                    ));

        }

        protected override DriverResult Editor(EventPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = CreateEditViewModel(part);
            updater.TryUpdateModel(model, Prefix, null, null);
            bool hasError = false;
            if (part.ContentItem.Id != 0)
            {

                if (model.StartDate == new DateTime())
                {
                    hasError = true;
                    updater.AddModelError("StartDate",
                        T("The event start date selected is invalid"));
                }
                if (model.EndDate == new DateTime())
                {
                    hasError = true;
                    updater.AddModelError("EndDate",
                        T("The event end date selected is invalid"));
                }
                if (string.IsNullOrWhiteSpace(model.SelectedEventCategoryIds))
                {
                    hasError = true;
                    updater.AddModelError("SelectedEventCategoryIds", T("You must assign an event category to this event"));
                }

                if (!string.IsNullOrWhiteSpace(model.ParentEventIdentifier))
                {
                    if (part.Identifier == model.ParentEventIdentifier)
                    {
                        hasError = true;
                        updater.AddModelError("ParentEventIdentifier", 
                            T("An event cannot be it's own Parent Event, select a different parent or remove the selected parent event."));
                    }
                    else if (_eventService.GetSubEvents(part).Any())
                    {
                        hasError = true;
                        updater.AddModelError("ParentEventIdentifier",
                            T("This event currently has sub-events associated with it.  A parent event cannot also be a sub-event."));
                    }
                }
                if (hasError)
                {
                    return ContentShape("Parts_Event_Edit",
                                            () => shapeHelper
                                                .EditorTemplate(
                                                    TemplateName: TemplateName,
                                                    Model: model,
                                                    Prefix: Prefix
                                                ));
                }

                _eventService.UpdateContentItemFromModel(part.ContentItem, model);
            }
            else
            {
                updater.AddModelError("Id",
                    T("Something went wrong.  There was no ContentItem Id assigned."));
            }
           // model.Categories = GetCategoryEntries(part);
           // model.EventAddresses = _addressService.GetEventAddresses().ToList();

            return ContentShape("Parts_Event_Edit",
                                    () => shapeHelper
                                        .EditorTemplate(
                                            TemplateName: TemplateName,
                                            Model: model,
                                            Prefix: Prefix
                                        ));
        }



        protected override void Importing(EventPart part, ImportContentContext context)
        {
            foreach (var prop in part.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x=>x.Name))
            {
                var data = context.Attribute(part.PartDefinition.Name, prop);
                if (data == null) continue;
                switch (prop)
                {
                    case "ParentEventIdentifier":
                        part.ParentEventIdentifier = data;
                        break;
                    case "Title":
                        part.As<TitlePart>().Title = data;
                        break;
                    case "StartDate":
                        part.StartDate = XmlConvert.ToDateTime(data, XmlDateTimeSerializationMode.Utc);
                        break;
                    case "EndDate":
                        part.EndDate = XmlConvert.ToDateTime(data, XmlDateTimeSerializationMode.Utc);
                        break;
                    case "TimeZone":
                        part.TimeZone = data;
                        break;
                    case "AllDayEvent":
                        part.AllDayEvent = Boolean.Parse(data);
                        break;
                    case "Description":
                        part.Description = data;
                        break;
                    case "AddressLocation":
                        part.AddressLocation = data;
                        break;
                    case "EventCategoryIdsValue":
                        _eventCategoryService.UpdateCategoriesForEvent(part.ContentItem, data.Split(',').Select(x => new CategoryEntry {Id = int.Parse(x), IsChecked = true}));
                        break;
                    case "RecurrenceId":
                        part.RecurrenceId = data;
                        break;
                    case "RecurrenceRule":
                        part.RecurrenceRule = data;
                        break;
                    case "RecurrenceException":
                        part.RecurrenceException = data;
                        break;
                    case "Url":
                        part.Url = data;
                        break;
                }
            }


        }

        protected override void Exporting(EventPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("ParentEventIdentifier", part.ParentEventIdentifier);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.Title);
            context.Element(part.PartDefinition.Name).SetAttributeValue("TimeZone", part.TimeZone);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AllDayEvent", part.AllDayEvent.ToString());
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AddressLocation", part.AddressLocation);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Url", part.Url);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EventCategoryIdsValue", string.Join(",", part.Categories.Select(x => x.Id)));

            context.Element(part.PartDefinition.Name).SetAttributeValue("RecurrenceId", part.RecurrenceId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("RecurrenceRule", part.RecurrenceRule);
            context.Element(part.PartDefinition.Name).SetAttributeValue("RecurrenceException", part.RecurrenceException);

            context.Element(part.PartDefinition.Name)
                .SetAttributeValue("StartDate", 
                XmlConvert.ToString(Convert.ToDateTime(part.StartDate), XmlDateTimeSerializationMode.Utc));

            context.Element(part.PartDefinition.Name)
                .SetAttributeValue("EndDate",
                XmlConvert.ToString(Convert.ToDateTime(part.EndDate), XmlDateTimeSerializationMode.Utc));

        }

        private EditEventViewModel CreateEditViewModel(EventPart part)
        {
            //var settings = part.PartDefinition.Settings.GetModel<SharedEventCategorySettings>();
            var identifiers = _eventCategoryService.GetEventCategoryIdentifiers(part.Categories.Select(x => x.Id));
            var data = new EditEventViewModel
            {
                ParentEventIdentifier = part.ParentEventIdentifier,
                Title = part.Title,
                AllDayEvent = part.AllDayEvent,
                StartDate = Convert.ToDateTime(part.StartDate),//TimeZoneHelper.ConvertTZIDFromUTC(part.StartDate,part.TimeZone),
                EndDate = Convert.ToDateTime(part.EndDate),//TimeZoneHelper.ConvertTZIDFromUTC(part.EndDate, part.TimeZone),
                Description = part.Description,
                Categories = GetCategoryEntries(part),
                SelectedEventCategoryIds = string.Join(",", identifiers),
                TimeZone = part.TimeZone,
                RecurrenceId = part.RecurrenceId,
                RecurrenceRule = part.RecurrenceRule,
                RecurrenceException = part.RecurrenceException,
                Url = part.Url,
                AddressLocation = part.AddressLocation
            };
            return data;
        }
        private List<CategoryEntry> GetCategoryEntries(EventPart part)
        {
            var categories = _eventCategoryService.GetEventCategories();

            var data = categories.Select(c => new CategoryEntry
            {
                ContentItem = c.ContentItem,
                Identifier = c.Get<IdentityPart>().Identifier,
                Id = c.Id,
                IsChecked = IsChecked(part, c.Id),
                Selectable = true,
                Name = c.CategoryName
            }).ToList();

            return data;
        }
        private bool IsChecked(EventPart part, int id)
        {
            if (part.Categories != null)
                return part.Categories.Any(x => x.Id == id);
            return false;
        }
    }
}