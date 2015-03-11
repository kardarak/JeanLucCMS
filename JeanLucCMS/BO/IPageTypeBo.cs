using System;

using JeanLucCMS.Models;

namespace JeanLucCMS.BO
{
    public interface IPageTypeBo
    {
        #region Methods

        Guid AddFolder(Guid? parentId);

        Guid AddPageType(Guid parentId);

        void Delete(Guid id);

        void Update(Guid id, PageTypePostModel model);

        #endregion
    }
}