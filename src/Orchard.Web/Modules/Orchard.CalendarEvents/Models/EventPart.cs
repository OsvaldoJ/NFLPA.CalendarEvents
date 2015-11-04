using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data.Conventions;

namespace Orchard.CalendarEvents.Models
{
    public class EventPartRecord : ContentPartRecord
    {
        public EventPartRecord()
        {
            Categories = new List<EventCategoryJoinRecord>();
        }
        public virtual string ParentEventIdentifier { get; set; }

        public virtual string Url { get; set; }
        public virtual string ImportUid { get; set; }

        [Required]
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }

        public virtual string TimeZone { get; set; }
        public virtual bool AllDayEvent { get; set; }

        /// <summary>
        /// The ID of the original event (the event Identifier)
        /// </summary>
        public virtual string RecurrenceId { get; set; }
        public virtual string RecurrenceRule { get; set; }
        public virtual string RecurrenceException { get; set; }

        [Required]
        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual bool ImportedFromGoogleCalendar { get; set; }

        public virtual string AddressLocation { get; set; }

        public virtual IList<EventCategoryJoinRecord> Categories { get; set; }

    }


    public class EventPart : ContentPart<EventPartRecord>
    {
        public string Identifier
        {
            get { return this.As<IdentityPart>().Identifier; }
        }

        public string ParentEventIdentifier
        {
            get { return Record.ParentEventIdentifier; }
            set { Record.ParentEventIdentifier = value; }
        }

        public string Title
        {
            get { return this.As<TitlePart>().Title; }
        }

        public string Url
        {
            get { return Record.Url; }
            set { Record.Url = value; }
        }

        public string ImportUid
        {
            get { return Record.ImportUid; }
            set { Record.ImportUid = value; }
        }

        [Required]
        public DateTime? StartDate
        {
            get { return Record.StartDate; }
            set { Record.StartDate = value; }
        }

        public DateTime? EndDate
        {
            get { return Record.EndDate; }
            set { Record.EndDate = value; }
        }

        public string TimeZone
        {
            get
            {
                var tz = Record.TimeZone;
                if (string.IsNullOrWhiteSpace(tz))
                    tz = "America/New_York";
                return tz;
            }
            set { Record.TimeZone = value; }
        }

        public bool AllDayEvent
        {
            get { return Record.AllDayEvent; }
            set { Record.AllDayEvent = value; }
        }

        /// <summary>
        /// The ID of the original event (the event Identifier)
        /// </summary>
        public string RecurrenceId
        {
            get { return Record.RecurrenceId; }
            set { Record.RecurrenceId = value; }
        }

        public string RecurrenceRule
        {
            get { return Record.RecurrenceRule; }
            set { Record.RecurrenceRule = value; }
        }

        public string RecurrenceException
        {
            get { return Record.RecurrenceException; }
            set { Record.RecurrenceException = value; }
        }

        [Required]
        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public bool ImportedFromGoogleCalendar
        {
            get { return Record.ImportedFromGoogleCalendar; }
            set { Record.ImportedFromGoogleCalendar = value; }
        }

        public string AddressLocation
        {
            get { return Record.AddressLocation; }
            set { Record.AddressLocation = value; }
        }

        public IEnumerable<CategoryPartRecord> Categories
        {
            get
            {
                return Record.Categories.Select(x => x.CategoryPartRecord);
            }
        }

        public DateTime? CreateUtc
        {
            get { return this.As<ICommonPart>().CreatedUtc; }
        }

        public DateTime? PublishedUtc
        {
            get { return this.As<ICommonPart>().PublishedUtc; }
        }

        public DateTime? ModifiedUtc
        {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }
    }
}