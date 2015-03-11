using System;
using System.Configuration;

namespace JeanLucCMS.Configuration
{
    public class SiteCollection : ConfigurationElementCollection
    {
        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteElement)element).UrlPrefix;
        }

        #endregion
    }
}