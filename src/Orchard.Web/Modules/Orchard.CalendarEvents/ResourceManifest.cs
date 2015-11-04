
using Orchard.UI.Resources;

namespace Orchard.CalendarEvents
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            //Global
            manifest.DefineStyle("Admin").SetUrl("GeneralAdminStyles.css");
            manifest.DefineStyle("CalendarStyles").SetUrl("CalendarStyles.css");
            
            //Moment.js - http://momentjs.com/docs/
            manifest.DefineScript("MomentJs").SetUrl("moment.min.js");

            manifest.DefineStyle("CategoryTags").SetUrl("CategoryTagStyles.css");
            
            manifest.DefineStyle("EditEvent").SetUrl("editEvent.css");
            manifest.DefineScript("EditEvent").SetUrl("editEvent.js").SetDependencies("jQuery");

            //Chosen - http://harvesthq.github.io/chosen/
            manifest.DefineStyle("Chosen").SetUrl("chosen.min.css");
            manifest.DefineScript("Chosen").SetUrl("chosen.jquery.min.js").SetDependencies("jQuery");
            manifest.DefineScript("CalendarChosen").SetUrl("CalendarChosen.js").SetDependencies("jQuery");

            //AngularBootstrap
            manifest.DefineScript("AngularBootstrapUI").SetUrl("ui-bootstrap-tpls-0.11.0.min.js")
                .SetDependencies("jQuery", "AngularJs");
            
            //KendoUI - http://docs.telerik.com/kendo-ui/api/introduction
            //Kendo Core https://github.com/telerik/kendo-ui-core
            //inlcudes datepicker and datetimepicker
            manifest.DefineScript("Kendo").SetUrl("kendo.ui.core.min.js").SetDependencies("jQuery", "AngularJS", "AngularBootstrapUI");
            manifest.DefineStyle("Kendo-Core-Default").SetUrl("kendo.common.core.min.css");
            manifest.DefineStyle("Kendo").SetUrl("kendo.default.min.css").SetDependencies("Kendo-Core-Default");

            //Full Calendar  http://arshaw.com/fullcalendar/
            manifest.DefineStyle("fullcalendar")
                .SetUrl("fullcalendar/fullcalendar.css");
            manifest.DefineScript("angular-ui-calendar").SetUrl("angular-ui/calendar.js").SetDependencies("AngularJs", "jQuery", "jQueryUI");
            manifest.DefineScript("fullcalendar").SetUrl("fullcalendar/fullcalendar.min.js")
                .SetDependencies("Bootstrap", "angular-ui-calendar");

            //XLSX - https://github.com/SheetJS/js-xlsx
            manifest.DefineScript("Blob").SetUrl("Blob.js").SetDependencies("jQuery");
            manifest.DefineScript("Filesaver").SetUrl("FileSaver.js").SetDependencies("Blob");
            manifest.DefineScript("XLSX").SetUrl("xlsx.full.min.js").SetDependencies("Filesaver");
            manifest.DefineStyle("DropZone").SetUrl("dropZone.css");

           //iCalendar Parsing
            manifest.DefineScript("iCalendar").SetUrl("jquery.icalendar.min.js").SetDependencies("jQuery");

            //EventSearch
            manifest.DefineScript("EventSearch").SetUrl("Angular_EventSearch.js")
                .SetDependencies("MomentJs", "jQuery", "AngularJS");
            

            // Google Maps API
            manifest.DefineScript("GoogleMapsPlacesApi")
                .SetUrl("https://maps.googleapis.com/maps/api/js?v=3&libraries=places");

        }
    }
}