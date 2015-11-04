using System.Web.Mvc;
using Orchard.CalendarEvents.Models;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using System;

namespace Orchard.CalendarEvents.Extensions {
    /// <summary>
    /// TODO: (PH:Autoroute) Many of these are or could be redundant (see controllers)
    /// </summary>
    public static class UrlHelperExtensions
    {
        #region Calendars
        public static string Calendars(this UrlHelper urlHelper) {
            return urlHelper.Action("List", "Calendar", new {area = "Orchard.CalendarEvents"});
        }

        public static string CalendarsForAdmin(this UrlHelper urlHelper) {
            return urlHelper.Action("List", "CalendarAdmin", new {area = "Orchard.CalendarEvents"});
        }

        public static string Calendar(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            var ap = calendarPart.ContentItem.Get(typeof (AutoroutePart)) as AutoroutePart;
            
            return ap != null && !string.IsNullOrWhiteSpace(ap.Path) ? 
                "/" + ap.Path : 
                urlHelper.Action("Item", "Calendar", new { calendarId = calendarPart.Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string CalendarForAdmin(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("Item", "CalendarAdmin", new { calendarId = calendarPart.Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string SchedulerEventsForCalendar(this UrlHelper urlHelper, string calendarIdentifier)
        {
            return urlHelper.Action("GetSchedulerEvents", "Calendar", new { calendarId = calendarIdentifier, area = "Orchard.CalendarEvents" });
        }

        public static string CalendarCreate(this UrlHelper urlHelper) {
            return urlHelper.Action("Create", "CalendarAdmin", new { area = "Orchard.CalendarEvents" });
        }

        public static string CalendarEdit(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("Edit", "CalendarAdmin", new { calendarId = calendarPart.Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string CalendarRemove(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("Remove", "CalendarAdmin", new { calendarId = calendarPart.Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string CalendarRemoveById(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("RemoveById", "CalendarAdmin", new { calendarId = calendarPart.Id, area = "Orchard.CalendarEvents" });
        }
        public static string CalendarSubscribe(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("Subscribe", "Calendar", new { calendarId = calendarPart.Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string CalendarSubscribeIcs(this UrlHelper urlHelper, CalendarPart calendarPart)
        {
            return urlHelper.Action("SubscribeIcs", "Calendar", new { calendarId = calendarPart.Identifier, fileName = calendarPart.Title, area = "Orchard.CalendarEvents" });
        }
        #endregion   
        #region Events

        public static string EventsForCalendarAdmin(this UrlHelper urlHelper, string identifier)
        {
            return urlHelper.Action("List", "EventAdmin", new { calendarId = identifier, area = "Orchard.CalendarEvents" });
        }
        public static string EventsForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string EventIdsForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("GetAllEventIds", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string RemoveEventsById(this UrlHelper urlHelper)
        {
            return urlHelper.Action("RemoveEventsById", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string Event(this UrlHelper urlHelper, EventPart eventPart)
        {
            return urlHelper.Action("Item", "Event", new { eventId = eventPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string Event(this UrlHelper urlHelper, string eventIdentifier)
        {
            return urlHelper.Action("Item", "Event", new { eventId = eventIdentifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventForAdmin(this UrlHelper urlHelper, EventPart eventPart)
        {
            return urlHelper.Action("Item", "EventAdmin", new { eventId = eventPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventForAdmin(this UrlHelper urlHelper, string eventIdentity)
        {
            return urlHelper.Action("Item", "EventAdmin", new { eventId = eventIdentity, area = "Orchard.CalendarEvents" });
        }

        public static string EventCreate(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }

        public static string EventEdit(this UrlHelper urlHelper, EventPart eventPart)
        {
            return urlHelper.Action("Edit", "EventAdmin", new { eventId = eventPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventEdit(this UrlHelper urlHelper, string eventIdentifier)
        {
            return urlHelper.Action("Edit", "EventAdmin", new { eventId = eventIdentifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventRemove(this UrlHelper urlHelper, EventPart eventPart)
        {
            return urlHelper.Action("Remove", "EventAdmin", new { eventId = eventPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string EventRemove(this UrlHelper urlHelper, string eventIdentifier)
        {
            return urlHelper.Action("Remove", "EventAdmin", new { eventId = eventIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string EventSubscribe(this UrlHelper urlHelper, EventPart eventPart)
        {
            return urlHelper.Action("Subscribe", "Event", new { eventId = eventPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string CreateEventFromScheduler(this UrlHelper urlHelper)
        {
            return urlHelper.Action("CreateEventFromScheduler", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string UpdateEventFromScheduler(this UrlHelper urlHelper)
        {
            return urlHelper.Action("UpdateEventFromScheduler", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string DeleteEventFromScheduler(this UrlHelper urlHelper)
        {
            return urlHelper.Action("DeleteEventFromScheduler", "EventAdmin", new { area = "Orchard.CalendarEvents" });
        }

        #endregion
        #region RSVP Forms

        public static string RsvpFormsForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "RsvpAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormItemForAdmin(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Item", "RsvpAdmin", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormItem(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Item", "Rsvp", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormCreate(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "RsvpAdmin", new { area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormCreateForEvent(this UrlHelper urlHelper, string eventIdentifier)
        {
            return urlHelper.Action("CreateForEvent", "RsvpAdmin", new { eventId = eventIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormEdit(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Edit", "RsvpAdmin", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormDelete(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Remove", "RsvpAdmin", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormViewResponse(this UrlHelper urlHelper, string rsvpResponseIdentifier)
        {
            return urlHelper.Action("Item", "RsvpResponse", new { rsvpResponseId = rsvpResponseIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpFormCreateResponse(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Create", "RsvpResponse", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }
        public static string RsvpResponseFormPreview(this UrlHelper urlHelper, string rsvpIdentifier)
        {
            return urlHelper.Action("Item", "RsvpResponse", new { rsvpId = rsvpIdentifier, area = "Orchard.CalendarEvents" });
        }

        #endregion
        #region Event Categories

        public static string EventCategoriesForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "EventCategoryAdmin", new { area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryForAdmin(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("Item", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryCreate(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "EventCategoryAdmin", new { area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryEdit(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("Edit", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryRemove(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("Remove", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryRemoveAllEvents(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("RemoveEventsForCategory", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }

        public static string EventCategoryRemoveAllEventsApi(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("RemoveEventsForCategoryApi", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string GetEventsForCategory(this UrlHelper urlHelper, CategoryPart eventCategoryPart)
        {
            return urlHelper.Action("GetEventsForCategory", "EventCategoryAdmin", new { categoryId = eventCategoryPart.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        #endregion
        #region Addresses

        public static string AddressesForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "AddressAdmin", new { area = "Orchard.CalendarEvents" });
        }
        
        public static string AddressCreate(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "AddressAdmin", new { area = "Orchard.CalendarEvents" });
        }
        
        public static string AddressRemove(this UrlHelper urlHelper, AddressPart part)
        {
            return urlHelper.Action("Remove", "AddressAdmin", new { id = part.Get<IdentityPart>().Identifier, area = "Orchard.CalendarEvents" });
        }
        public static string RenderAddressDisplay(this UrlHelper urlHelper, string addressId)
        {
            return urlHelper.Action("RenderEventAddressDisplay", "AddressAdmin", new { id = addressId, area = "Orchard.CalendarEvents" });
        }

        #endregion
    }
}