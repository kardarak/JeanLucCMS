using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    public class CmsActionSelector : IHttpActionSelector
    {
        #region Membres IHttpActionSelector

        public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            throw new NotImplementedException();
        }

        public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var owinContext = controllerContext.Request.Properties["MS_OwinContext"] as IOwinContext;
            var methodInfo = owinContext == null ? null : owinContext.Request.Get<MethodInfo>("cms-method");

            if (methodInfo == null)
            {
                return null;
            }

            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, methodInfo);
        }

        #endregion
    }
}