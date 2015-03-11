using System;
using System.IO;

using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace JeanLucCMS.Tools
{
    internal class Razor : IRazor
    {
        public ITemplateService GetAdminService()
        {
            var viewPathTemplate = "JeanLucCMS.Views.{0}";

            var templateConfig = new TemplateServiceConfiguration();
            templateConfig.Resolver = new DelegateTemplateResolver(
                name =>
                {
                    var resourcePath = string.Format(viewPathTemplate, name);
                    var stream = typeof(HtmlActionResult).Assembly.GetManifestResourceStream(resourcePath);

                    if (stream == null)
                    {
                        return null;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                });
            var service = new TemplateService(templateConfig);
            service.AddNamespace("System.Web.Routing");

            return service;
        }

        public ITemplateService GetCmsService()
        {
            var templateConfig = new TemplateServiceConfiguration();
            templateConfig.Resolver = new DelegateTemplateResolver(
                name =>
                {
                    var viewsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views");
                    var viewFile = new FileInfo(Path.Combine(viewsRoot, name + ".cshtml"));

                    if (!viewFile.Exists)
                    {
                        viewFile = new FileInfo(Path.Combine(viewsRoot, "Shared" , name + ".cshtml"));
                    }

                    if (!viewFile.Exists)
                    {
                        return null;
                    }
                    
                    using (var reader = new StreamReader(viewFile.OpenRead()))
                    {
                        return reader.ReadToEnd();
                    }
                });
            var service = new TemplateService(templateConfig);
            service.AddNamespace("System.Web.Routing");

            return service;
        }
    }
}