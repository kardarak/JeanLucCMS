using System;
using System.Collections.Generic;

using JeanLucCMS.Configuration;
using JeanLucCMS.Entity;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    public interface IGlobalCache
    {
        IList<Entity.ControllerAction> ControllerActions { get; }
        Dictionary<string, byte[]> AdminContent { get; }
        CmsSection Configuration { get; }
        IList<Model> Models { get; }
        string AdminRoot { get; }

        void ResetPages();

        PageData GetPageByUrl(PathString url);
        IEnumerable<PageData> GetPageById(Guid pageId);
    }
}