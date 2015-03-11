using System;
using System.Configuration;

namespace JeanLucCMS.Configuration
{
    public class NamespaceCollection : ConfigurationElementCollection
    {
        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new NamespaceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NamespaceElement)element).NameSpace;
        }

        #endregion
    }
}