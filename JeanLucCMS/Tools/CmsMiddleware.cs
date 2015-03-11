using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    internal class CmsMiddleware : OwinMiddleware
    {
        #region Fields

        private readonly IGlobalCache _globalCache;
        private readonly IRazor _razor;
        private readonly IDictionary<string, HttpMethod> _httpMethods = new Dictionary<string, HttpMethod>()
        {
            { "GET", HttpMethod.Get },
            { "POST", HttpMethod.Post },
            { "PUT", HttpMethod.Put },
            { "DELETE", HttpMethod.Delete }
        }; 

        #endregion

        #region Constructors

        public CmsMiddleware(OwinMiddleware next, IGlobalCache globalCache, IRazor razor)
            : base(next)
        {
            this._globalCache = globalCache;
            this._razor = razor;
        }

        #endregion

        #region Methods

        public override async Task Invoke(IOwinContext context)
        {
            var path = context.Request.Path.ToString();
            var adminPath = "/" + this._globalCache.Configuration.AdminPath;

            if (path.StartsWith(adminPath, StringComparison.OrdinalIgnoreCase))
            {
                // if no one took the admin request
                await this.Show404(context);
            }
            else
            {
                var page = this._globalCache.GetPageByUrl(context.Request.Path);

                if (page != null)
                {
                    var action = this._globalCache.ControllerActions.FirstOrDefault(
                        a => a.ControllerType.FullName == page.ControllerType &&
                             a.ActionName == page.ActionName);

                    context.Request.Set("cms-page", page);
                    context.Request.Set("cms-controller", action == null ? null : action.ControllerType);

                    if (action != null && this._httpMethods.ContainsKey(context.Request.Method) && action.Methods.ContainsKey(this._httpMethods[context.Request.Method]))
                    {
                        context.Request.Set("cms-method", action.Methods[this._httpMethods[context.Request.Method]]);
                    }
                }

                await this.Next.Invoke(context);
            }
        }

        protected async Task Show404(IOwinContext context)
        {
            var page404 = new HtmlActionResult(this._razor.GetAdminService(), this._globalCache, "Shared.404.cshtml", null, "text/html", null, null);
            var response = await page404.ExecuteAsync(new CancellationToken());

            using (var writer = new StreamWriter(context.Response.Body))
            {
                context.Response.Headers["Content-Type"] = "text/html";
                context.Response.StatusCode = 404;

                writer.Write(await response.Content.ReadAsStringAsync());
            }
        }

        #endregion
    }
}