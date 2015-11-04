using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Orchard.CalendarEvents
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission EventSuperAdmin =
            new Permission { Description = "Event Super Administrator - can delete all events and categories from the system and perform other bulk operations.", Name = "EventSuperAdmin" };
        public static readonly Permission ManageEventTypes =
            new Permission { Description = "Manage Event Types", Name = "ManageEventTypes" };
        public static readonly Permission ManageEvents =
            new Permission { Description = "Manage Events", Name = "ManageEvents" };
        public static readonly Permission ManageCalendars =
            new Permission { Description = "Manage Calendars", Name = "ManageCalendars", ImpliedBy = new[] { ManageEvents } };
        public static readonly Permission ManageStates =
            new Permission { Description = "Manage States", Name = "ManageStates", ImpliedBy = new[] { ManageEvents } };
        public static readonly Permission ManageAddresses =
            new Permission { Description = "Manage Addresses", Name = "ManageAddresses", ImpliedBy = new[] { ManageEvents } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                EventSuperAdmin,
                ManageEvents,
                ManageCalendars,
                ManageEventTypes,
                ManageStates,
                ManageAddresses
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {EventSuperAdmin, ManageEvents,ManageEventTypes, ManageAddresses}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {ManageEvents}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new Permission[0] 
                }
            };
        }

    }
}
