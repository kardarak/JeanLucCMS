using System;
using System.Collections.Generic;

namespace JeanLucCMS.Models
{
    public class PagePostModel
    {
        #region Properties

        public OperationType Operation { get; set; }

        public Guid? PageTypeId { get; set; }

        public List<PageLanguageModel> Languages { get; set; }

        #endregion
    }
}