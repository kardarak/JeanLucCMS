using System;

namespace JeanLucCMS.Fields
{
    public class HtmlField : BaseField
    {
        #region Constructors

        public HtmlField(Type resourceType, string resourceKey)
            : base(resourceType, resourceKey)
        {
        }

        #endregion

        #region Methods

        public override string GetEditor(string fullFieldName, object value)
        {
            return "<textarea class='html-editor' name='" + fullFieldName + "' style='width:100%'>" + value + "</textarea>";
        }

        #endregion
    }
}