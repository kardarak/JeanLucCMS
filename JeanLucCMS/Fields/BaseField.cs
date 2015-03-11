using System;
using System.Globalization;
using System.Resources;

using Microsoft.Practices.Unity;

namespace JeanLucCMS.Fields
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseField : Attribute
    {
        #region Fields

        private readonly string _resourceKey;
        private readonly ResourceManager _resourceManager;

        #endregion

        #region Constructors

        protected BaseField(Type resourceType, string resourceKey)
        {
            this._resourceManager = new ResourceManager(resourceType);
            this._resourceKey = resourceKey;
        }

        #endregion

        #region Properties

        public int Order { get; set; }

        #endregion

        #region Methods

        public virtual void SetInjection(IUnityContainer unityContainer)
        {
        }

        public abstract string GetEditor(string fullFieldName, object value);

        public string GetFieldTitle(CultureInfo culture)
        {
            return this._resourceManager.GetString(this._resourceKey, culture);
        }

        #endregion
    }
}