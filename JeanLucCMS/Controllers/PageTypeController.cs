using System;
using System.Web.Http;

using JeanLucCMS.BO;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

namespace JeanLucCMS.Controllers
{
    public class PageTypeController : ApiController
    {
        #region Fields

        private const string ControllerName = "PageType";
        private readonly IGlobalCache _globalCache;
        private readonly IRazor _razor;
        private readonly IPageTypeBo _pageTypeBo;

        #endregion

        #region Constructors

        public PageTypeController(IGlobalCache globalCache, IRazor razor, IPageTypeBo pageTypeBo)
        {
            this._globalCache = globalCache;
            this._razor = razor;
            this._pageTypeBo = pageTypeBo;
        }

        #endregion

        #region Methods

        [HttpGet]
        public IHttpActionResult Edit(Guid id)
        {
            var model = new PageTypeModel(this._globalCache, id);

            if (model.Item == null)
            {
                return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = null as Guid? });
            }

            return new HtmlActionResult(this._razor.GetAdminService(), this._globalCache, "PageType.Index.cshtml", model, "text/html", ControllerName, "Edit");
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            var model = new PageTypeModel(this._globalCache);

            return new HtmlActionResult(this._razor.GetAdminService(), this._globalCache, "PageType.Index.cshtml", model, "text/html", ControllerName, "Index");
        }

        [HttpPost]
        public IHttpActionResult Post(PageTypePostModel model)
        {
            return Post(null, model);
        }

        [HttpPost]
        public IHttpActionResult Post(Guid? id, PageTypePostModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            switch (model.Operation)
            {
                case OperationType.AddFolder:
                    var newId = this._pageTypeBo.AddFolder(id);

                    return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = newId });
                case OperationType.AddItem:
                    if (id == null)
                    {
                        throw new ArgumentNullException("id");
                    }

                    var newId2 = this._pageTypeBo.AddPageType(id.Value);
                    return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = newId2 });
                case OperationType.Update:
                    if (id == null)
                    {
                        throw new ArgumentNullException("id");
                    }

                    this._pageTypeBo.Update(id.Value, model);

                    return this.Edit(id.Value);
                case OperationType.Delete:
                    if (id == null)
                    {
                        throw new ArgumentNullException("id");
                    }

                    this._pageTypeBo.Delete(id.Value);
                    return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = null as Guid? });
                default:
                    throw new InvalidOperationException(model.Operation + " is not supported");
            }
        }

        #endregion
    }
}