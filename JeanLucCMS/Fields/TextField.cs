using System;

namespace JeanLucCMS.Fields
{
    public class TextField : BaseField
    {
        #region Constructors

        public TextField(Type resourceType, string resourceKey)
            : base(resourceType, resourceKey)
        {
        }

        #endregion

        #region Methods

        public override string GetEditor(string fullFieldName, object value)
        {
            return "<input type='text' name='" + fullFieldName + "' value='" + value + "' />";
        }

        #endregion
    }
}