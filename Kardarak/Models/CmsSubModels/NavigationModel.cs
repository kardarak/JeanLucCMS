using System;

using JeanLucCMS.Fields;

using Kardarak.Resources;

namespace Kardarak.Models.CmsSubModels
{
    public class NavigationModel
    {
        [TextField(typeof(GeneralModelTexts), "PageTitle", Order = 1)]
        public string PageTitle { get; set; }

        [BoolField(typeof(GeneralModelTexts), "IncludeInNavigation", Order = 2)]
        public bool IncludeInNavigation { get; set; }

        [TextField(typeof(GeneralModelTexts), "NavigationTitle", Order = 3)]
        public string NavigationTitle { get; set; }
    }
}