using System;
using System.Data;
using Orchard.CalendarEvents.Models;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentPermissions.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Title.Models;
using Orchard.Data.Migration;

namespace Orchard.CalendarEvents {
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {

            // Record Definition
            SchemaBuilder.CreateTable("CategoryPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("CategoryName", column => column.WithLength(255))
                    .Column<string>("Description", column => column.WithLength(500))
                );

            SchemaBuilder.CreateTable("CalendarPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("ShortDescription", column => column.WithLength(500))
                    .Column("Description", DbType.String, column => column.Unlimited())
                    .Column<string>("FeedProxyUrl")
                );

            SchemaBuilder.CreateTable("EventPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<DateTime>("StartDate")
                    .Column<DateTime>("EndDate")
                    .Column("Description", DbType.String, column => column.Unlimited())
                    .Column<string>("TimeZone")
                    .Column<bool>("AllDayEvent")
                    .Column<string>("RecurrenceId")
                    .Column<string>("RecurrenceRule")
                    .Column<string>("RecurrenceException")
                    .Column<bool>("ImportedFromGoogleCalendar")
                    .Column<string>("ImportUid")
                    .Column<string>("Url")
                    .Column<string>("AddressLocation")
                    .Column<string>("ParentEventIdentifier")
                );


            SchemaBuilder.CreateTable("CalendarCategoryJoinRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CategoryPartRecord_Id")
                    .Column<int>("CalendarPartRecord_Id")
                );

            SchemaBuilder.CreateTable("EventCategoryJoinRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CategoryPartRecord_Id")
                    .Column<int>("EventPartRecord_Id")
                );

            // Part Definition
            ContentDefinitionManager.AlterPartDefinition(
                typeof (AddressPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterPartDefinition(
                typeof(EventPart).Name, cfg =>
                    cfg.Attachable(false)
                    .WithField("EventImage", field => field
                        .OfType("MediaLibraryPickerField")
                        .WithDisplayName("Event Image")
                        .WithSetting("MediaLibraryPickerFieldSettings.Required", "false")
                        .WithSetting("MediaLibraryPickerFieldSettings.Multiple", "false"))
                        .WithSetting("MediaLibraryPickerFieldSettings.Hint", "Select a main image for the event: 778px x 250px")
                );
            
            ContentDefinitionManager.AlterTypeDefinition(
                "Event", cfg => cfg
                    .WithPart(typeof(TitlePart).Name)
                    .WithPart(typeof(EventPart).Name)
                    .WithPart(typeof(CommonPart).Name)
                    .WithPart(typeof(IdentityPart).Name)
                    .WithPart(typeof(AutoroutePart).Name)
                    .WithPart(typeof(ContentPermissionsPart).Name)
                    .DisplayedAs("Calendar Event Item")
                    .WithPart(typeof(AutoroutePart).Name, builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: 'events/{Content.Slug}', Description: 'my-projections'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                );


            // Type Definition
            ContentDefinitionManager.AlterTypeDefinition(
                "Calendar", cfg => cfg
                    .WithPart(typeof(TitlePart).Name)
                    .WithPart(typeof(CalendarPart).Name)
                    .WithPart(typeof(CommonPart).Name)
                    .WithPart(typeof(IdentityPart).Name)
                     .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-projections'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("ContentPermissionsPart")
                    .DisplayedAs("Event Calendar")
                );

            ContentDefinitionManager.AlterTypeDefinition(
                "CalendarWidget", cfg => cfg
                    .WithPart(typeof(CalendarWidgetPart).Name)
                    .WithPart(typeof(CommonPart).Name)
                    .WithPart(typeof(IdentityPart).Name)
                    .WithPart(typeof(ContentPermissionsPart).Name)
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                    .DisplayedAs("Event Calendar Widget")
                );

            ContentDefinitionManager.AlterTypeDefinition(
                "EventCategory", cfg => cfg
                    .WithPart(typeof(CategoryPart).Name)
                    .WithPart(typeof(IdentityPart).Name)
                    .WithPart(typeof(CommonPart).Name)
                    .DisplayedAs("Event Category")
                );

            return 1;
        }
    }
}