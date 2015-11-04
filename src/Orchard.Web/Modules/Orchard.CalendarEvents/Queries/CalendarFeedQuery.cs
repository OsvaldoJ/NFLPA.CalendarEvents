using System;
using System.Web.Mvc;
using System.Xml.Linq;
using JetBrains.Annotations;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.Services;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Feeds;
using Orchard.Core.Feeds.Models;
using Orchard.Core.Feeds.StandardBuilders;
using Orchard.Mvc.Extensions;

namespace Orchard.CalendarEvents.Queries
{
    [UsedImplicitly]
    public class CalendarFeedQuery : IFeedQueryProvider, IFeedQuery
    {
        private readonly IContentManager _contentManager;
        private readonly ICalendarService _calendarService;

        public CalendarFeedQuery(IContentManager contentManager, ICalendarService calendarService)
        {
            _contentManager = contentManager;
            _calendarService = calendarService;
        }

        public FeedQueryMatch Match(FeedContext context)
        {
            var containerIdValue = context.ValueProvider.GetValue("calendar");
            if (containerIdValue == null)
                return null;

            return new FeedQueryMatch { FeedQuery = this, Priority = -5 };
        }
        public void Execute(FeedContext context)
        {
            var calendarId = context.ValueProvider.GetValue("calendar");
            if (calendarId == null)
                return;

            var containerId = (int)calendarId.ConvertTo(typeof(int));
            var container = _contentManager.Get<CalendarPart>(containerId);

            if (container == null)
            {
                return;
            }

            var inspector = new ItemInspector(container, _contentManager.GetItemMetadata(container));
            if (context.Format == "rss")
            {
                var link = new XElement("link");
                context.Response.Element.SetElementValue("title", inspector.Title);
                context.Response.Element.Add(link);
                context.Response.Element.SetElementValue("description", inspector.Description);

                context.Response.Contextualize(requestContext =>
                {
                    var urlHelper = new UrlHelper(requestContext);
                    var uriBuilder = new UriBuilder(urlHelper.MakeAbsolute("/"))
                    {
                        Path = urlHelper.RouteUrl(inspector.Link) ?? "/"
                    };
                    link.Add(uriBuilder.Uri.OriginalString);
                });
            }
            else
            {
                context.Builder.AddProperty(context, null, "title", inspector.Title);
                context.Builder.AddProperty(context, null, "description", inspector.Description);
                context.Response.Contextualize(requestContext =>
                {
                    var urlHelper = new UrlHelper(requestContext);
                    context.Builder.AddProperty(context, null, "link", urlHelper.RouteUrl(inspector.Link));
                });
            }

            var items = _calendarService.GetEvents(container.As<IdentityPart>().Identifier);

            foreach (var item in items)
            {
                // call item.ContentItem to force a cast to ContentItem, and 
                // thus use CorePartsFeedItemBuilder
                context.Builder.AddItem(context, item.ContentItem);
            }
        }
    }
}