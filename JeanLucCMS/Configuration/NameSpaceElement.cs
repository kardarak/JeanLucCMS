using System;
using System.Configuration;

namespace JeanLucCMS.Configuration
{
    public class NamespaceElement : ConfigurationElement
    {
        #region Properties

        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get
            {
                return this["assembly"].ToString();
            }
            set
            {
                this["assembly"] = value;
            }
        }

        [ConfigurationProperty("namespace", IsKey = true, IsRequired = true)]
        public string NameSpace
        {
            get
            {
                return (string)this["namespace"];
            }
            set
            {
                this["namespace"] = value;
            }
        }

        #endregion
    }
}