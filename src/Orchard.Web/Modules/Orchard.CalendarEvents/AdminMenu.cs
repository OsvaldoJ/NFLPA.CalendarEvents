using Orchard.CalendarEvents;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Orchard.CalendarEvents
{
    public class AdminMenu : INavigationProvider
    {
        public string MenuName
        {
            get
            {
                return "admin";
            }
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Events Management"), "4", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {

            menu
                .Add(T("Events"), "1.0",
                    item => item.Action("Events", "Admin", new { area = "Orchard.CalendarEvents" })
                        .Permission(Permissions.ManageEvents));
            menu
                .Add(T("Calendars"), "1.1",
                    item => item.Action("Calendars", "Admin", new { area = "Orchard.CalendarEvents" })
                        .Permission(Permissions.ManageCalendars));
            menu
                .Add(T("Events"), "1.2",
                    item => item.Action("Events", "Admin", new { area = "Orchard.CalendarEvents" })
                        .Permission(Permissions.ManageEvents));
            menu
                .Add(T("Event Categories"), "1.4",
                    item => item.Action("EventCategories", "Admin", new { area = "Orchard.CalendarEvents" })
                        .Permission(Permissions.ManageEventTypes));

        }
    }
}