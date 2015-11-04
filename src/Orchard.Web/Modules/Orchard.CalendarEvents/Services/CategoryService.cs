using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Orchard.CalendarEvents.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;

namespace Orchard.CalendarEvents.Services
{
    public interface ICategoryService : IDependency
    {
        ContentItem Get(string id);
        IEnumerable<CategoryPart> GetEnabledEventCategories();
        IEnumerable<CategoryPart> GetEventCategories();
        IEnumerable<CategoryPart> GetEventCategoriesByIdList(IEnumerable<string> ids);
        IEnumerable<CategoryPart> GetEventCategoriesByCalendar(ContentItem item);
        IEnumerable<CategoryPart> GetEventCategoriesByEvent(ContentItem item);
        IEnumerable<int> GetEventsByCalendar(ContentItem item);
        IEnumerable<int> GetEventsByCategoryIds(IEnumerable<string> ids);
        IEnumerable<CategoryPart> GetEventCategoriesByNameSnippet(string snippet, int maxCount = 10);
        CategoryPart GetEventCategoriesByName(string tagName);
        CategoryPart CreateCategory(string tagName, bool isDisabled);
        void Delete(ContentItem item);
        void UpdateCategoriesForCalendar(ContentItem item, IEnumerable<CategoryEntry> categories);
        void UpdateCategoriesForEvent(ContentItem item, IEnumerable<CategoryEntry> categories);
        void UpdateCategoriesForCalendar(ContentItem item, IEnumerable<string> categoriesIds);
        void UpdateCategoriesForEvent(ContentItem item, IEnumerable<string> categoriesIds);
        IEnumerable<string> GetEventCategoryIdentifiers(IEnumerable<int> categoryIds);
    }
    public class CategoryService : ICategoryService
    {

        private readonly IContentManager _contentManager;
        private readonly IRepository<CalendarPartRecord> _calendarPartRecordRepository;
        private readonly IRepository<EventPart> _eventPartRecordRepository;
        private readonly IRepository<EventCategoryJoinRecord> _eventCategoryJoinRecordRepository;
        private readonly IRepository<CalendarCategoryJoinRecord> _calendarCategoryJoinRecordRepository;
        private readonly ISessionLocator _sessionLocator;


        public CategoryService(IContentManager contentManager, 
            IRepository<CalendarPartRecord> calendarPartRecordRepository, 
            IRepository<CategoryPartRecord> categoryPartRecordRepository, 
            IRepository<EventPart> eventPartRecordRepository, 
            IRepository<EventCategoryJoinRecord> eventCategoryJoinRecordRepository, 
            IRepository<CalendarCategoryJoinRecord> calendarCategoryJoinRecordRepository, ISessionLocator sessionLocator)
        {
            _contentManager = contentManager;
            _calendarPartRecordRepository = calendarPartRecordRepository;
            _eventPartRecordRepository = eventPartRecordRepository;
            _eventCategoryJoinRecordRepository = eventCategoryJoinRecordRepository;
            _calendarCategoryJoinRecordRepository = calendarCategoryJoinRecordRepository;
            _sessionLocator = sessionLocator;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ContentItem Get(string id)
        {
            var itemPart = GetEventCategories().SingleOrDefault(x=>x.As<IdentityPart>().Identifier == id);
            return itemPart == null ? null : itemPart.ContentItem;
        }

        public IEnumerable<CategoryPart> GetEnabledEventCategories()
        {
            return _contentManager
                .Query<CategoryPart>(VersionOptions.Published)
                .Join<IdentityPartRecord>()
                .Join<CategoryPartRecord>()
                .List<CategoryPart>()
                .OrderBy(x=>x.CategoryName);
        }
        public IEnumerable<CategoryPart> GetEventCategories()
        {
            return _contentManager
                .Query<CategoryPart>(VersionOptions.Latest)
                .Join<IdentityPartRecord>()
                .Join<CategoryPartRecord>()
                .List<CategoryPart>()
                .OrderBy(x => x.CategoryName);
        }

        public IEnumerable<CategoryPart> GetEventCategoriesByIdList(IEnumerable<string> ids)
        {
            var categories = _contentManager.Query<CategoryPart>()
                .Join<IdentityPartRecord>()
                .Join<CategoryPartRecord>()
                .Where<IdentityPartRecord>(x => ids.Contains(x.Identifier)).List();
            return categories;
        }

        public IEnumerable<CategoryPart> GetEventCategoriesByNameSnippet(string snippet, int maxCount = 10)
        {
            return String.IsNullOrEmpty(snippet) ? null :
                GetEventCategories().Where(tag => tag.CategoryName.StartsWith(snippet)).Take(maxCount);
        }
        
        public CategoryPart GetEventCategoriesByName(string tagName)
        {
            return _contentManager
                .Query(VersionOptions.Latest)
                .Join<IdentityPartRecord>()
                .Join<CategoryPartRecord>()
                .Where<CategoryPartRecord>(tag => tag.CategoryName == tagName)
                .List<CategoryPart>()
                .FirstOrDefault();
        }

        public CategoryPart CreateCategory(string tagName, bool isDisabled)
        {
            var result = GetEventCategoriesByName(tagName);
            if (result == null)
            {
                result = new CategoryPart
                {
                    CategoryName = tagName
                };
                _contentManager.Create(result);
            }
            return result;
        }

        public void Delete(ContentItem item)
        {
            _contentManager.Remove(item);
        }

        public void UpdateCategoriesForCalendar(ContentItem item, IEnumerable<CategoryEntry> categories)
        {
            var record = item.As<CalendarPart>().Record;
            var oldRewards = _calendarCategoryJoinRecordRepository.Fetch(
                r => r.CalendarPartRecord == record);

            var lookupNew = categories
                .Where(e => e.IsChecked)
                .Select(e => e.ContentItem.As<CategoryPart>().Record)
                .ToDictionary(r => r, r => false);

            foreach (var joinRecord in oldRewards)
            {
                if (lookupNew.ContainsKey(joinRecord.CategoryPartRecord))
                {

                    lookupNew[joinRecord.CategoryPartRecord] = true;
                }
                else
                {
                    _calendarCategoryJoinRecordRepository.Delete(joinRecord);
                }
            }

            foreach (var category in lookupNew.Where(kvp => !kvp.Value).Select(kvp => kvp.Key))
            {
                _calendarCategoryJoinRecordRepository.Create(new CalendarCategoryJoinRecord
                {
                    CalendarPartRecord = record,
                    CategoryPartRecord = category
                });
            }
        }
        public void UpdateCategoriesForCalendar(ContentItem item, IEnumerable<string> categoriesIds)
        {
            var categories = _contentManager.Query<CategoryPart>()
                .Where<IdentityPartRecord>(x => categoriesIds.Contains(x.Identifier))
                .List()
                .Select(x => new CategoryEntry { ContentItem = x.ContentItem, IsChecked = true });

            UpdateCategoriesForCalendar(item, categories);
        }

        public void UpdateCategoriesForEvent(ContentItem item, IEnumerable<CategoryEntry> categories)
        {
            var record = item.As<EventPart>().Record;
            var oldRewards = _eventCategoryJoinRecordRepository.Fetch(
                r => r.EventPartRecord == record);

            var lookupNew = categories
                .Where(e => e.IsChecked)
                .Select(e => e.ContentItem.As<CategoryPart>().Record)
                .ToDictionary(r => r, r => false);

            foreach (var joinRecord in oldRewards)
            {
                if (lookupNew.ContainsKey(joinRecord.CategoryPartRecord))
                {

                    lookupNew[joinRecord.CategoryPartRecord] = true;
                }
                else
                {
                    _eventCategoryJoinRecordRepository.Delete(joinRecord);
                }
            }

            foreach (var category in lookupNew.Where(kvp => !kvp.Value).Select(kvp => kvp.Key))
            {
                var joinRecord = new EventCategoryJoinRecord();
                joinRecord.EventPartRecord = record;
                joinRecord.CategoryPartRecord = category;
                _eventCategoryJoinRecordRepository.Create(joinRecord);
                
            }
            _eventCategoryJoinRecordRepository.Flush();
        }
        public void UpdateCategoriesForEvent(ContentItem item, IEnumerable<string> categoriesIds)
        {
            var categories = _contentManager.Query<CategoryPart>()
                .Join<IdentityPartRecord>()
                .Where<IdentityPartRecord>(x => categoriesIds.Contains(x.Identifier))
                .List()
                .Select(x => new CategoryEntry { ContentItem = x.ContentItem, IsChecked = true });

            UpdateCategoriesForEvent(item, categories);
        }

        public IEnumerable<string> GetEventCategoryIdentifiers(IEnumerable<int> categoryIds)
        {
            categoryIds = categoryIds.ToList();
            if (!categoryIds.Any())
                return new List<string>();

            var identifiers = _contentManager
                .GetMany<CategoryPart>(categoryIds, VersionOptions.Latest, QueryHints.Empty)
                .Select(x => x.Identifier);
            return identifiers;
        }

        public IEnumerable<CategoryPart> GetEventCategoriesByCalendar(ContentItem item)
        {
            var categoryIds = _calendarCategoryJoinRecordRepository.Table.Where(x => x.CalendarPartRecord.Id == item.Id).Select(x => x.CategoryPartRecord.Id);
            var categories = _contentManager.GetMany<CategoryPart>(categoryIds, VersionOptions.Latest, QueryHints.Empty);
            return categories;
        }

        public IEnumerable<int> GetEventsByCalendar(ContentItem item)
        {
            var categoryIds = _calendarCategoryJoinRecordRepository.Table.Where(x => x.CalendarPartRecord.Id == item.Id).Select(x => x.CategoryPartRecord.Id);
            var eventIds = _eventCategoryJoinRecordRepository.Table.Where(x => categoryIds.Contains(x.CategoryPartRecord.Id)).Select(x => x.EventPartRecord.Id);
            return eventIds;
        }



        public IEnumerable<CategoryPart> GetEventCategoriesByEvent(ContentItem item)
        {
            var eventCategoryIds = _eventCategoryJoinRecordRepository.Table.Where(x => x.EventPartRecord.Id == item.Id).Select(x => x.CategoryPartRecord.Id);
            var categories = _contentManager.GetMany<CategoryPart>(eventCategoryIds, VersionOptions.Latest, QueryHints.Empty);
            return categories;
        }

        public IEnumerable<int> GetEventsByCategoryIds(IEnumerable<string> ids)
        {
            var categoryIds = _contentManager.Query<CategoryPart>()
                .Join<IdentityPartRecord>()
                .Where<IdentityPartRecord>(x => ids.Contains(x.Identifier))
                .List()
                .Select(x => x.Id);
            var eventIds = _eventCategoryJoinRecordRepository.Table.Where(x => categoryIds.Contains(x.CategoryPartRecord.Id)).Select(x => x.EventPartRecord.Id);
            return eventIds;
        }
    }
}