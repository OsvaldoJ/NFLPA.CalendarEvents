using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.CalendarEvents.Models;

namespace Orchard.CalendarEvents.ViewModels
{
    public class EditEventViewModel
    {
        public string ParentEventIdentifier { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllDayEvent { get; set; }
        public string TimeZone { get; set; }
        public string Description { get; set; }

        [Required]
        [DisplayName("Event Categories Included")]
        public string SelectedEventCategoryIds { get; set; }
        public List<CategoryEntry> Categories { get; set; }
        public string RecurrenceId { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }

        public string Url { get; set; }

        #region Address
        public string AddressLocation { get; set; }
        #endregion
    }

}