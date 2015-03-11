using System;
using System.Collections.Generic;

using JeanLucCMS.Models;

namespace JeanLucCMS.Factory
{
    public interface IContentEditorFactory
    {
        IList<ContentEditorModel> GetContentEditors(Type modelType);
    }
}