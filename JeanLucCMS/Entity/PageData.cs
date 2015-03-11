using System;
using System.Linq;

using JeanLucCMS.Tools;

namespace JeanLucCMS.Entity
{
    public class PageData
    {
        #region Constructeurs

        internal PageData(Page page, PageLanguage language, PageType pageType, IGlobalCache cache)
        {
            var langContent = language;

            if (!string.IsNullOrEmpty(language.CopyFromLanguage))
            {
                langContent = page.PageLanguages.FirstOrDefault(l => language.CopyFromLanguage.Equals(l.Language, StringComparison.OrdinalIgnoreCase))
                       ?? language;
            }

            this.Id = page.PageId;
            this.Language = language.Language;
            this.PageContent = langContent.ContentJson;
            this.ParentId = page.ParentPageId;
            this.NameUrl = language.NameUrl;
            this.ControllerType = pageType == null ? null : pageType.ControllerType;
            this.ActionName = pageType == null ? null : pageType.ActionName;

            var model = pageType == null ? null : cache.Models.FirstOrDefault(m => m.Name == pageType.ModelType);

            this.ModelType = model == null ? null : model.Class;
        }

        #endregion

        #region Propriétés

        public string ActionName { get; private set; }
        public string ControllerType { get; private set; }

        public Guid Id { get; private set; }
        public string Language { get; private set; }
        public Type ModelType { get; private set; }
        public string PageContent { get; private set; }
        public Guid? ParentId { get; private set; }
        public string Path { get; internal set; }
        internal string NameUrl { get; private set; }

        #endregion
    }
}