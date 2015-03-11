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
    public class AdminControllerActivator : IHttpControllerActivator
    {
        #region Fields

        private readonly IUnityContainer _adminContainer;

        #endregion

        #region Constructors

        internal AdminControllerActivator(IUnityContainer adminContainer)
        {
            this._adminContainer = adminContainer;
        }

        #endregion

        #region IHttpControllerActivator Members

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = this._adminContainer.Resolve(controllerType) as IHttpController;
            
            return controller;
        }

        #endregion
    }
}