using System;

using JeanLucCMS;

using Microsoft.Practices.Unity;

using Owin;

namespace Kardarak
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseJeanLucCms(new UnityContainer());
            app.UseHandlerAsync((req, res) =>
            {
                res.ContentType = "text/plain";
                res.StatusCode = 404;
                return res.WriteAsync("404");
            });
        }
    }
}