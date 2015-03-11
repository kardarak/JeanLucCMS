using System;
using System.Web.Http;

using JeanLucCMS.Entity;
using JeanLucCMS.Tools;

using Newtonsoft.Json;

namespace JeanLucCMS
{
    public abstract class BaseController : ApiController
    {
        #region Properties

        public string Content
        {
            get
            {
                return this.PageData == null ? null : this.PageData.PageContent;
            }
        }

        internal PageData PageData { private get; set; }

        internal IRazor Razor { get; set; }

        #endregion

        #region Methods

        protected T GetContent<T>() where T : BaseModel
        {
            return this.PageData.PageContent == null || this.PageData.ModelType == null
                ? null
                : JsonConvert.DeserializeObject(this.PageData.PageContent, this.PageData.ModelType) as T;
        }

        protected IHttpActionResult View(string viewName, object model)
        {
            var viewResult = new HtmlActionResult(this.Razor.GetCmsService(), viewName, model);

            return viewResult;
        }

        #endregion
    }
}