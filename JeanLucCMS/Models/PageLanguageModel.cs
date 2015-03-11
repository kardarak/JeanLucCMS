using System;

namespace JeanLucCMS.Models
{
    public class PageLanguageModel
    {
        #region Properties
        
        public Guid? PageTypeId { get; set; }
        public LanguagePageType LanguagePageType { get; set; }
        public string Language { get; set; }
        public string NameUrl { get; set; }
        public string UseLanguage { get; set; }
        public object Content { get; set; }

        #endregion
    }
}