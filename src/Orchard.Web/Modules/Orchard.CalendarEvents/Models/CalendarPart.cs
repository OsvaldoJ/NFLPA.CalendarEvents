using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data.Conventions;

namespace Orchard.CalendarEvents.Models
{
    public class CalendarPartRecord : ContentPartRecord
    {
        public CalendarPartRecord()
        {
            Categories = new List<CalendarCategoryJoinRecord>();
        }
        public virtual string ShortDescription { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual string FeedProxyUrl { get; set; }
        public virtual IList<CalendarCategoryJoinRecord> Categories { get; set; }
    }

    public class CalendarPart : ContentPart<CalendarPartRecord>
    {
        public string Title
        {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Identifier
        {
            get { return this.As<IdentityPart>().Identifier; }
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

        public string ShortDescription
        {
            get { return Record.ShortDescription; }
            set { Record.ShortDescription = value; }
        }

        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public string FeedProxyUrl
        {
            get { return Record.FeedProxyUrl; }
            set { Record.FeedProxyUrl = value; }
        }

        public IEnumerable<CategoryPartRecord> Categories
        {
            get { return Record.Categories.Select(x => x.CategoryPartRecord); }
        }
        
    }
}