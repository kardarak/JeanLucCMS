using System;

using JeanLucCMS.Models;

namespace JeanLucCMS.BO
{
    public interface IPageBo
    {
        #region Methods

        Guid AddPage(Guid? parentId);

        void Delete(Guid id);

        void Update(Guid id, PagePostModel model);

        #endregion
    }
}