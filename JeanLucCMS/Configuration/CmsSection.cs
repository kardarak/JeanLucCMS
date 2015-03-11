using System;
using System.Configuration;

namespace JeanLucCMS.Configuration
{
    public class CmsSection : ConfigurationSection
    {
        #region Properties

        [ConfigurationProperty("adminPath", DefaultValue = "false", IsRequired = true)]
        public string AdminPath
        {
            get
            {
                return (string)this["adminPath"];
            }
            set
            {
                this["adminPath"] = value;
            }
        }

        [ConfigurationProperty("controllers", IsRequired = true, IsDefaultCollection = true)]
        public NamespaceCollection Controllers
        {
            get
            {
                return (NamespaceCollection)this["controllers"];
            }
            set
            {
                this["controllers"] = value;
            }
        }

        [ConfigurationProperty("models", IsRequired = true, IsDefaultCollection = true)]
        public NamespaceCollection Models
        {
            get
            {
                return (NamespaceCollection)this["models"];
            }
            set
            {
                this["models"] = value;
            }
        }

        [ConfigurationProperty("sites", IsRequired = true, IsDefaultCollection = true)]
        public SiteCollection Sites
        {
            get
            {
                return (SiteCollection)this["sites"];
            }
            set
            {
                this["sites"] = value;
            }
        }

        #endregion
    }
}