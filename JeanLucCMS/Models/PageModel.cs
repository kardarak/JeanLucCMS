using System;
using System.Collections.Generic;
using System.Linq;

using JeanLucCMS.Configuration;
using JeanLucCMS.Entity;
using JeanLucCMS.Factory;
using JeanLucCMS.Fields;
using JeanLucCMS.Tools;

using Newtonsoft.Json;

namespace JeanLucCMS.Models
{
    public class PageModel
    {
        #region Fields

        private readonly IGlobalCache _globalCache;
        private readonly IContentEditorFactory _contentEditorFactory;

        #endregion

        #region Constructors

        internal PageModel(IGlobalCache globalCache, IContentEditorFactory contentEditorFactory)
            : this(globalCache, contentEditorFactory, Guid.Empty)
        {
        }

        internal PageModel(IGlobalCache globalCache, IContentEditorFactory contentEditorFactory, Guid id)
        {
            this._globalCache = globalCache;
            this._contentEditorFactory = contentEditorFactory;

            using (var context = new CmsEntities())
            {
                this.Pages = this.GetPages(context).ToList();
                var page = context.Pages.FirstOrDefault(i => i.PageId == id);

                if (id != Guid.Empty)
                {
                    this.Item = this.CreateModel(page);
                }

                Func<PageType, string, string> recursiveName = null;

                recursiveName =
                    (pageType, name) => pageType == null ? name : recursiveName(pageType.Parent, pageType.Name + "." + name);

                this.PageTypes = context.PageTypes.Where(pt => !pt.IsFolder).ToList().Select(
                    pt =>
                        new KeyValuePair<string, Guid>(recursiveName(pt, null), pt.PageTypeId)).ToList();

                if (this.Item != null)
                {
                    this.ContentEditors = this.GetContentEditors(page == null ? null : page.PageType);
                }
            }
        }

        #endregion

        #region Properties

        public IList<ContentEditorModel> ContentEditors { get; private set; }
        public PageItemModel Item { get; set; }
        public IList<KeyValuePair<string, Guid>> PageTypes { get; private set; }
        public IList<PageItemModel> Pages { get; private set; }

        #endregion

        #region Methods

        private PageItemModel CreateModel(Page page)
        {
            if (page == null)
            {
                return null;
            }

            var model = new PageItemModel()
            {
                Id = page.PageId,
                Name = page.PageLanguages.Any() ? page.PageLanguages.First().NameUrl : "Empty " + (page.PageType == null ? "page" : page.PageType.Name),
                ParentId = page.ParentPageId,
                PageTypeId = page.PageTypeId
            };

            var contentType = page.PageType == null
                ? null
                : this._globalCache.Models.Where(m => m.Name == page.PageType.ModelType).Select(m => m.Class).FirstOrDefault();

            foreach (SiteElement site in this._globalCache.Configuration.Sites)
            {
                var languageModel = new PageLanguageModel()
                {
                    Language = site.Language,
                    LanguagePageType = LanguagePageType.NoPage
                };

                var language = page.PageLanguages.FirstOrDefault(l => l.Language == site.Language);

                if (language != null)
                {
                    languageModel.NameUrl = language.NameUrl;

                    if (string.IsNullOrWhiteSpace(language.CopyFromLanguage))
                    {
                        languageModel.LanguagePageType = LanguagePageType.LanguageDefined;
                    }
                    else
                    {
                        languageModel.LanguagePageType = LanguagePageType.UseOtherLanguage;
                        languageModel.UseLanguage = language.CopyFromLanguage;
                    }
                }

                if (contentType != null)
                {
                    if (language != null && !string.IsNullOrWhiteSpace(language.ContentJson))
                    {
                        try
                        {
                            languageModel.Content = JsonConvert.DeserializeObject(language.ContentJson, contentType) as BaseModel;
                        }
                        catch (JsonSerializationException)
                        {
                            // Ignore JSON error
                        }
                    }

                    if (languageModel.Content == null)
                    {
                        languageModel.Content = Activator.CreateInstance(contentType) as BaseModel;
                    }
                }

                model.Languages.Add(languageModel);
            }

            return model;
        }

        private IList<ContentEditorModel> GetContentEditors(PageType pageType)
        {
            var result = new List<ContentEditorModel>();

            if (this.Item != null && pageType != null)
            {
                var model = this._globalCache.Models.FirstOrDefault(m => m.Name == pageType.ModelType);
                if (model != null)
                {
                    result.AddRange(this._contentEditorFactory.GetContentEditors(model.Class));
                }
            }

            return result;
        }

        private IEnumerable<PageItemModel> GetPages(CmsEntities context)
        {
            var items = context.Pages.ToDictionary(page => page.PageId, this.CreateModel);

            foreach (var item in items.Values.Where(i => i.ParentId.HasValue))
            {
                items[item.ParentId.Value].Childrens.Add(item);
            }

            return items.Values.Where(i => i.ParentId == null);
        }

        #endregion
    }
}