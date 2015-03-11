using System;

namespace JeanLucCMS.Fields
{
    public class BoolField : BaseField
    {
        #region Constructors

        public BoolField(Type resourceType, string resourceKey)
            : base(resourceType, resourceKey)
        {
        }

        #endregion

        #region Methods

        public override string GetEditor(string fullFieldName, object value)
        {
            var boolean = false;

            bool.TryParse(value == null ? string.Empty : value.ToString(), out boolean);

            return "<input type='checkbox' name='" + fullFieldName + "' value = 'true' " + (boolean ? "checked" : string.Empty) + " />";
        }

        #endregion
    }
}