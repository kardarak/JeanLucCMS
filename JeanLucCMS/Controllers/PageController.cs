using System;
using System.Web.Http;

using JeanLucCMS.BO;
using JeanLucCMS.Factory;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

namespace JeanLucCMS.Controllers
{
    public class PageController : ApiController
    {
        #region Fields

        private const string ControllerName = "Page";
        private readonly IGlobalCache _globalCache;
        private readonly IContentEditorFactory _contentEditorFactory;
        private readonly IRazor _razor;
        private readonly IPageBo _pageBo;
        private readonly IPageTypeBo _pageTypeBo;

        #endregion

        #region Constructors

        public PageController(IGlobalCache globalCache, IContentEditorFactory contentEditorFactory, IRazor razor, IPageBo pageBo, IPageTypeBo pageTypeBo)
        {
            this._globalCache = globalCache;
            this._contentEditorFactory = contentEditorFactory;
            this._razor = razor;
            this._pageBo = pageBo;
            this._pageTypeBo = pageTypeBo;
        }

        #endregion

        #region Methods

        [HttpGet]
        public IHttpActionResult Edit(Guid id)
        {
            var model = new PageModel(this._globalCache, this._contentEditorFactory,id);

            if (model.Item == null)
            {
                return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = null as Guid? });
            }

            return new HtmlActionResult(this._razor.GetAdminService(), this._globalCache, "Page.Index.cshtml", model, "text/html", ControllerName, "Edit");
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            var model = new PageModel(this._globalCache, this._contentEditorFactory);

            return new HtmlActionResult(this._razor.GetAdminService(), this._globalCache, "Page.Index.cshtml", model, "text/html", ControllerName, "Index");
        }

        [HttpPost]
        public IHttpActionResult Post(PagePostModel model)
        {
            return this.Post(null, model);
        }

        [HttpPost]
        public IHttpActionResult Post(Guid? id, PagePostModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            switch (model.Operation)
            {
                case OperationType.AddItem:
                    var newId = this._pageBo.AddPage(id);
                    this._globalCache.ResetPages();

                    return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = newId });
                case OperationType.Update:
                    if (id == null)
                    {
                        throw new ArgumentNullException("id");
                    }

                    this._pageBo.Update(id.Value, model);
                    this._globalCache.ResetPages();

                    return this.Edit(id.Value);
                case OperationType.Delete:
                    if (id == null)
                    {
                        throw new ArgumentNullException("id");
                    }

                    this._pageBo.Delete(id.Value);
                    this._globalCache.ResetPages();

                    return this.RedirectToRoute("AdminPages", new { controller = ControllerName, id = null as Guid? });
                default:
                    throw new InvalidOperationException(model.Operation + " is not supported");
            }
        }

        #endregion
    }
}