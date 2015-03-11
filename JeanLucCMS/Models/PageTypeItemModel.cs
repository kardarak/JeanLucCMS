using System;
using System.Collections.Generic;

namespace JeanLucCMS.Models
{
    public class PageTypeItemModel
    {
        #region Constructors

        public PageTypeItemModel()
        {
            this.Childrens = new List<PageTypeItemModel>();
        }

        #endregion

        #region Properties

        public string ActionFullName { get; set; }

        public IList<PageTypeItemModel> Childrens { get; private set; }

        public Guid Id { get; set; }
        public bool IsFolder { get; set; }
        public string ModelType { get; set; }
        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        #endregion
    }
}