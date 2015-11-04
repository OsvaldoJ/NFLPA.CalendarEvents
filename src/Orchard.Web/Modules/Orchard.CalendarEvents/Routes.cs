using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Orchard.CalendarEvents {
    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {

            #region Calendars
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Calendars/Create",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "CalendarAdmin"},
                                                {"action", "Create"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Calendars/{calendarId}/Edit",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "CalendarAdmin"},
                                                {"action", "Edit"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Calendars/{calendarId}/Remove",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "CalendarAdmin"},
                                                {"action", "Remove"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Calendars/{calendarId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "CalendarAdmin"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Calendars",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "CalendarAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Calendars/{calendarId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Calendar"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Calendars/{calendarId}/Subscribe",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Calendar"},
                                                {"action", "Subscribe"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Calendars/{calendarId}/Subscribe/{fileName}.ics",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Calendar"},
                                                {"action", "SubscribeIcs"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
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
#endregion

            #region Events

            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Events/Create",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "Create"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Events/{eventId}/Edit",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "Edit"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Events/{eventId}/Remove",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "Remove"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Events/{eventId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Events",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventsForCalendar/{calendarId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
                                                 
            new RouteDescriptor {
                Route = new Route(
                    "EventsById/{eventId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Event"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "EventsById/{eventId}/Subscribe",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "Event"},
                                                {"action", "Subscribe"}
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
                    "Admin/Orchard.CalendarEvents/EventCategories/Create",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "Create"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventCategories/{categoryId}/Edit",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "Edit"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventCategories/{categoryId}/Remove",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "Remove"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventCategories/{categoryId}/RemoveEvents",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "RemoveEventsForCategory"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventCategories/{categoryId}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/EventCategories",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "EventCategoryAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            #endregion                                   
          
            #region Addresses

            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses/Create",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "Create"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses/{id}/Edit",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "Edit"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses/{id}/Remove",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "Remove"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses/{id}",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "Item"}
                                            },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/Addresses/{id}/RenderEventAddressDisplay",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "List"}
                                            },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"}
                                            },
                    new MvcRouteHandler())
            },
            new RouteDescriptor {
                Route = new Route(
                    "Admin/Orchard.CalendarEvents/AddressAdmin/RenderEventAddressDisplay",
                    new RouteValueDictionary {
                                                {"area", "Orchard.CalendarEvents"},
                                                {"controller", "AddressAdmin"},
                                                {"action", "List"}
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