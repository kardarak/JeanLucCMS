using System;
using System.Collections.Generic;

namespace JeanLucCMS.Models
{
    public class PageItemModel
    {
        #region Constructors

        public PageItemModel()
        {
            this.Childrens = new List<PageItemModel>();
            this.Languages = new List<PageLanguageModel>();
        }

        #endregion

        #region Properties

        public IList<PageItemModel> Childrens { get; private set; }
        public IList<PageLanguageModel> Languages { get; private set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? PageTypeId { get; set; }

        #endregion
    }
}