using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

using JeanLucCMS.BO;
using JeanLucCMS.Factory;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

using Microsoft.Practices.Unity;

using Owin;

using Unity.WebApi;

namespace JeanLucCMS
{
    public static class AppBuilderExtensions
    {
        #region Methods

        public static void UseJeanLucCms(this IAppBuilder app, IUnityContainer containerCms)
        {
            var containerAdmin = CreateAdminUnityContainer();
            var globalCache = containerAdmin.Resolve<IGlobalCache>();

            // Admin pages
            app.UseWebApi(CreateWebApiAdminConfig(globalCache, containerAdmin));
            app.Use<AdminContentMiddleware>(globalCache);

            // Pages configured in the CMS
            app.Use<CmsMiddleware>(globalCache, containerAdmin.Resolve<IRazor>());
            app.UseWebApi(CreateWebApiCmsConfig(containerCms, containerAdmin));
        }

        private static IUnityContainer CreateAdminUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IGlobalCache, GlobalCache>(new HierarchicalLifetimeManager());
            container.RegisterType<IPageTypeBo, PageTypeBo>(new HierarchicalLifetimeManager());
            container.RegisterType<IPageBo, PageBo>(new HierarchicalLifetimeManager());
            container.RegisterType<IRazor, Razor>(new HierarchicalLifetimeManager());
            container.RegisterType<IContentEditorFactory, ContentEditorFactory>(new HierarchicalLifetimeManager());

            return container;
        }

        private static HttpConfiguration CreateWebApiAdminConfig(IGlobalCache globalCache, IUnityContainer adminContainer)
        {
            var config = new HttpConfiguration();
            RegisterAdminRoutes(config, globalCache);
            config.Services.Replace(typeof(IHttpControllerActivator), new AdminControllerActivator(adminContainer));
            config.ParameterBindingRules.Insert(0, typeof(PagePostModel), param => param.BindWithModelBinding(new PagePostModelBinder(globalCache, param)));

            return config;
        }

        private static HttpConfiguration CreateWebApiCmsConfig(IUnityContainer cmsContainer, IUnityContainer adminContainer)
        {
            var config = new HttpConfiguration();
            config.Services.Replace(typeof(IHttpControllerSelector), new CmsControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new CmsActionSelector());
            config.Services.Replace(typeof(IHttpControllerActivator), new CmsControllerActivator(cmsContainer, adminContainer));

            config.Routes.MapHttpRoute(
                name: "CmsMVC",
                routeTemplate: "{*route}",
                defaults: new { controller = RouteParameter.Optional }
                );

            return config;
        }

        private static void RegisterAdminRoutes(HttpConfiguration config, IGlobalCache globalCache)
        {
            config.Routes.MapHttpRoute(
                name: "AdminPages",
                routeTemplate: globalCache.Configuration.AdminPath + "/{controller}/{id}",
                defaults: new { controller = "Page", id = RouteParameter.Optional }
                );
        }

        #endregion
    }
}