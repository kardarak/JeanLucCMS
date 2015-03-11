using System;

using JeanLucCMS;
using JeanLucCMS.Fields;

using Kardarak.Models.CmsSubModels;
using Kardarak.Resources;

namespace Kardarak.Models.Cms
{
    public class StandardPageModel : BaseModel
    {
        /// <summary>
        /// Default contructor (Must create all sub models)
        /// </summary>
        public StandardPageModel()
        {
            this.Navigation = new NavigationModel();
        }

        #region Properties

        [SubModelField(typeof(GeneralModelTexts), "Navigation", Order = 1)]
        public NavigationModel Navigation { get; set; }

        [HtmlField(typeof(GeneralModelTexts), "PageContent", Order = 2)]
        public string PageContent { get; set; }

        /// <summary>
        /// Text from the controller (Not configured for the page in the DB)
        /// </summary>
        public string TextTest { get; set; }

        #endregion
    }
}