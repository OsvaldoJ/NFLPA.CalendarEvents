using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Orchard.CalendarEvents
{
    public class ApiRoutes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {

            #region Calendars

            new RouteDescriptor 
            {
                Route = new Route(
                    "api/Calendars/{calendarId}/GetSchedulerEvents",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Calendar"},
                                                {"action", "GetSchedulerEvents"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            
            new RouteDescriptor 
            {
                Route = new Route(
                    "api/Calendars/{calendarId}/Events",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Calendar"},
                                                {"action", "GetEvents"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
#endregion

            #region Events

            new RouteDescriptor {
                Route = new Route(
                    "api/Events/CreateEventFromScheduler",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "CreateEventFromScheduler"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/UpdateEventFromScheduler",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "UpdateEventFromScheduler"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/DeleteEventFromScheduler",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "DeleteEventFromScheduler"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/Search",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "Search"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/SearchCalendar/{calendarId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "SearchCalendar"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/GetEventByIdentifier/{eventIdentifier}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "GetEventByIdentifier"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/GetAllEventIds",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "GetAllEventIds"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/RemoveAllEvents",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "RemoveAllEventsApi"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/Events/RemoveEventsById",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "RemoveEventsById"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
#endregion
          
            #region Event Categories

            new RouteDescriptor {
                Route = new Route(
                    "api/Events/GetEventCategories",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "GetEventCategoriesJson"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/EventCategories/{categoryId}/GetEvents",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "GetEventsForCategory"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/EventCategories/{categoryId}/RemoveEvents",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "RemoveEventsForCategory"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },

#endregion                                   
          
            #region Addresses

#endregion                                   

            #region Rsvp
                             
            new RouteDescriptor {
                Route = new Route(
                    "api/RsvpForm/GetRsvpFormsForEvent/{eventId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "RsvpAdmin"},
                                                {"action", "GetRsvpFormsJson"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/RsvpForm/GetRsvpForms",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Rsvp"},
                                                {"action", "GetRsvpFormsJson"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "api/RsvpForm/GetRsvpFormsAdmin",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "RsvpAdmin"},
                                                {"action", "GetRsvpFormsJson"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            }

#endregion

            };
        }
    }
}