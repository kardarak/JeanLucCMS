using System;
using System.Configuration;

namespace JeanLucCMS.Configuration
{
    public class SiteElement : ConfigurationElement
    {
        #region Properties

        [ConfigurationProperty("language", IsKey = true, IsRequired = true)]
        public string Language
        {
            get
            {
                return (string)this["language"];
            }
            set
            {
                this["language"] = value;
            }
        }

        [ConfigurationProperty("rootPage", IsRequired = true)]
        public Guid RootPage
        {
            get
            {
                return (Guid)this["rootPage"];
            }
            set
            {
                this["rootPage"] = value;
            }
        }

        [ConfigurationProperty("urlPrefix", IsKey = true, IsRequired = true)]
        public string UrlPrefix
        {
            get
            {
                return (string)this["urlPrefix"];
            }
            set
            {
                this["urlPrefix"] = value;
            }
        }

        #endregion
    }
}