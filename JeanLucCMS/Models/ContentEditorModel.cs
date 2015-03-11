using System;
using System.Globalization;
using System.Reflection;

using JeanLucCMS.Factory;
using JeanLucCMS.Fields;

using Microsoft.Practices.Unity;

namespace JeanLucCMS.Models
{
    public class ContentEditorModel
    {
        #region Fields

        private readonly BaseField _field;
        private readonly int _order;
        private readonly string _title;
        private readonly string _propertyName;

        #endregion

        #region Constructors

        public ContentEditorModel(PropertyInfo property, BaseField field, IUnityContainer container)
        {
            this._field = field;
            this._order = field.Order;
            this._title = field.GetFieldTitle(CultureInfo.CurrentUICulture);
            this._propertyName = property.Name;

            this._field.SetInjection(container);
        }

        #endregion

        #region Properties

        public int Order
        {
            get
            {
                return this._order;
            }
        }

        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
        }

        public string Title
        {
            get
            {
                return this._title;
            }
        }

        #endregion

        #region Methods

        public string GetEditor(string fullModelName, object model)
        {
            if (model == null)
            {
                return this._field.GetEditor(fullModelName + "." + this.PropertyName, null);
            }

            var property = model.GetType().GetProperty(this.PropertyName);
            var value = property == null ? null : property.GetValue(model);

            return this._field.GetEditor(fullModelName + "." + this.PropertyName, value);
        }

        #endregion
    }
}