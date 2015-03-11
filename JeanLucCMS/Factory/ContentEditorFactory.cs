using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JeanLucCMS.Entity;
using JeanLucCMS.Fields;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

using Microsoft.Practices.Unity;

namespace JeanLucCMS.Factory
{
    public class ContentEditorFactory : IContentEditorFactory
    {
        private readonly IUnityContainer _container;

        public ContentEditorFactory(IUnityContainer container)
        {
            this._container = container;
        }
        public IList<ContentEditorModel> GetContentEditors(Type modelType)
        {
            return (from property in modelType.GetProperties()
                        let attribute = property.GetCustomAttributes(typeof(BaseField), true).FirstOrDefault()
                        where attribute != null
                    select new ContentEditorModel(property, attribute as BaseField, this._container)).ToList();
        }
    }
}
