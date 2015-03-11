using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

using JeanLucCMS.Entity;

using Microsoft.Owin;
using Microsoft.Practices.Unity;

namespace JeanLucCMS.Tools
{
    public class CmsControllerActivator : IHttpControllerActivator
    {
        #region Fields

        private readonly IUnityContainer _cmsContainer;
        private readonly IUnityContainer _adminContainer;

        #endregion

        #region Constructors

        internal CmsControllerActivator(IUnityContainer cmsContainer, IUnityContainer adminContainer)
        {
            this._cmsContainer = cmsContainer;
            this._adminContainer = adminContainer;
        }

        #endregion

        #region IHttpControllerActivator Members

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var owinContext = request.Properties["MS_OwinContext"] as IOwinContext;
            var pageData = owinContext == null ? null : owinContext.Request.Get<PageData>("cms-page");

            var controller = this._cmsContainer.Resolve(controllerType) as BaseController;

            if (controller != null)
            {
                controller.Razor = this._adminContainer.Resolve<IRazor>();
                controller.PageData = pageData;
            }

            return controller;
        }

        #endregion
    }
}