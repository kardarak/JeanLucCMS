using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    internal class CmsControllerSelector : IHttpControllerSelector
    {
        #region Fields

        private readonly HttpConfiguration _configuration;

        #endregion

        #region Constructors

        public CmsControllerSelector(HttpConfiguration configuration)
        {
            this._configuration = configuration;
        }

        #endregion

        #region IHttpControllerSelector Members

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var owinContext = request.Properties["MS_OwinContext"] as IOwinContext;
            var controller = owinContext == null ? null : owinContext.Request.Get<Type>("cms-controller");

            if (controller == null)
            {
                return null;
            }

            return new HttpControllerDescriptor(this._configuration, controller.FullName, controller);
        }

        #endregion
    }
}