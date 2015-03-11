using System;
using System.Linq;
using System.Text;

using JeanLucCMS.Factory;

using Microsoft.Practices.Unity;

namespace JeanLucCMS.Fields
{
    public class SubModelField : BaseField
    {
        private IContentEditorFactory contentEditorFactory;

        #region Constructors

        public SubModelField(Type resourceType, string resourceKey)
            : base(resourceType, resourceKey)
        {
        }

        #endregion

        #region Methods

        public override void SetInjection(IUnityContainer unityContainer)
        {
            if (this.contentEditorFactory == null)
            {
                this.contentEditorFactory = unityContainer.Resolve<IContentEditorFactory>();

                base.SetInjection(unityContainer);
            }
        }

        public override string GetEditor(string fullFieldName, object value)
        {
            if (value == null)
            {
                return null;
            }

            var result = new StringBuilder();

            foreach (var editor in this.contentEditorFactory.GetContentEditors(value.GetType()).OrderBy(e => e.Order))
            {
                result.AppendLine("<div class='panel panel-default'>");
                result.AppendLine("    <div class='panel-heading'>" + editor.Title+ "</div>");
                result.AppendLine("    <div class='panel-body'>");
                result.AppendLine(editor.GetEditor(fullFieldName , value));
                result.AppendLine("    </div>");
                result.AppendLine("</div>");
            }

            return result.ToString();
        }

        #endregion
    }
}