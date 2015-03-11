using System;

using RazorEngine.Templating;

namespace JeanLucCMS.Tools
{
    public interface IRazor
    {
        #region Methods

        ITemplateService GetAdminService();

        ITemplateService GetCmsService();

        #endregion
    }
}