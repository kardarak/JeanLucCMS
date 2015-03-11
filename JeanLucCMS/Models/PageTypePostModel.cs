using System;

namespace JeanLucCMS.Models
{
    public class PageTypePostModel
    {
        #region Properties

        public string ActionFullName { get; set; }

        public string ModelType { get; set; }
        public string Name { get; set; }
        public OperationType Operation { get; set; }

        #endregion
    }
}