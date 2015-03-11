using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using RazorEngine.Templating;

namespace JeanLucCMS.Tools
{
    internal class HtmlActionResult : IHttpActionResult
    {
        #region Champs

        private readonly ITemplateService _templateService;
        private readonly IGlobalCache _globalCache;
        private readonly string _template;
        private readonly object _model;
        private readonly string _contentType;
        private readonly string _controller;
        private readonly string _action;
        private readonly bool _isAdmin;

        #endregion

        #region Constructeurs

        public HtmlActionResult(ITemplateService templateService, string view, object model)
        {
            this._isAdmin = false;
            this._templateService = templateService;
            this._template = view;
            this._model = model;
            this._contentType = "text/html";
        }

        public HtmlActionResult(
            ITemplateService templateService,
            IGlobalCache globalCache,
            string template,
            object model,
            string contentType,
            string controller,
            string action)
        {
            this._isAdmin = true;
            this._templateService = templateService;
            this._globalCache = globalCache;
            this._template = template;
            this._model = model;
            this._contentType = contentType;
            this._controller = controller;
            this._action = action;
        }

        #endregion

        #region Membres IHttpActionResult

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var executeContext = new ExecuteContext();

            if (this._isAdmin)
            {
                executeContext.ViewBag.Controller = this._controller;
                executeContext.ViewBag.Action = this._action;
                executeContext.ViewBag.AdminRoot = this._globalCache.AdminRoot;

            }

            var template = this._templateService.Resolve(this._template, this._model);

            if (template == null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(template.Run(executeContext));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(this._contentType);
            return Task.FromResult(response);
        }

        #endregion
    }
}