using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;

namespace Orchard.CalendarEvents.Models
{
    public class CategoryPartRecord : ContentPartRecord
    {
        public virtual string CategoryName { get; set; }
        public virtual string Description { get; set; }
    }

    public class CategoryPart : ContentPart<CategoryPartRecord>
    {
        public string Identifier
        {
            get { return this.As<IdentityPart>().Identifier; }
        }

        [Required(ErrorMessage = "*"), DisplayName("Category Name")]
        public string CategoryName
        {
            get { return Record.CategoryName; }
            set { Record.CategoryName = value; }
        }

        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

    }
}