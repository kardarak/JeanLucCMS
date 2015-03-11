using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;

using JeanLucCMS.Configuration;
using JeanLucCMS.Entity;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    internal class GlobalCache : IGlobalCache
    {
        #region Fields

        private const string RootNamespace = "JeanLucCMS";
        private const string FormatRootPath = "/{0}/";
        private const string NamespaceAdminContent = RootNamespace + ".Content";

        private Dictionary<string, byte[]> _adminContent;
        private CmsSection _configuration;
        private List<Model> _models;
        private List<Entity.ControllerAction> _actions;
        private string _adminRoot;

        private IEnumerable<PageData> _pages;
        private Dictionary<string, PageData> _pagesByUrl;
        private Dictionary<Guid, IList<PageData>> _pagesById;

        #endregion

        #region Properties

        private IEnumerable<PageData> Pages
        {
            get
            {
                return this._pages ?? (this._pages = this.GetPages());
            }
        }

        #endregion

        #region IGlobalCache Members

        public Dictionary<string, byte[]> AdminContent
        {
            get
            {
                if (this._adminContent == null)
                {
                    var resources =
                        this.GetType()
                            .Assembly.GetManifestResourceNames()
                            .Where(r => r.StartsWith(NamespaceAdminContent, StringComparison.OrdinalIgnoreCase));

                    this._adminContent = resources.ToDictionary(i => i.Substring(RootNamespace.Length + 1), this.GetResource);
                }

                return this._adminContent;
            }
        }

        public string AdminRoot
        {
            get
            {
                return this._adminRoot
                       ?? (this._adminRoot = string.Format(CultureInfo.InvariantCulture, FormatRootPath, this.Configuration.AdminPath).Replace("//", "/"));
            }
        }

        public CmsSection Configuration
        {
            get
            {
                if (this._configuration == null)
                {
                    var configuration = (CmsSection)ConfigurationManager.GetSection("cmsSection");

                    if (string.IsNullOrWhiteSpace(configuration.AdminPath))
                    {
                        throw new ConfigurationErrorsException("'adminPath' must be specified and cannot be empty");
                    }

                    this._configuration = configuration;
                }

                return this._configuration;
            }
        }

        public IList<Entity.ControllerAction> ControllerActions
        {
            get
            {
                if (this._actions == null)
                {
                    foreach (NamespaceElement modelTarget in this.Configuration.Controllers)
                    {
                        if (string.IsNullOrWhiteSpace(modelTarget.Assembly))
                        {
                            throw new ConfigurationErrorsException("'assembly' must be specified and cannot be empty");
                        }

                        if (string.IsNullOrWhiteSpace(modelTarget.NameSpace))
                        {
                            throw new ConfigurationErrorsException("'nameSpace' must be specified and cannot be empty");
                        }

                        var assembly = Assembly.Load(modelTarget.Assembly);
                        var target = modelTarget;
                        var classes =
                            assembly.GetTypes()
                                .Where(t => t.Namespace != null && t.Namespace.StartsWith(target.NameSpace, StringComparison.OrdinalIgnoreCase));
                        var methods = classes.SelectMany(
                            item => item.GetMethods().Where(m => m.DeclaringType != typeof(object) && m.IsPublic).Select(
                                m =>
                                    new
                                    {
                                        controllerType = item,
                                        method = m,
                                        action = m.Name,
                                        httpMethods = m.GetCustomAttributes().OfType<IActionHttpMethodProvider>().Select(a => a.HttpMethods)
                                    }));

                        this._actions = (from method in methods
                            from httpMethods in method.httpMethods
                            from httpMethod in httpMethods
                            select new { method, httpMethod })
                            .GroupBy(m => new { m.method.controllerType, m.method.action }).Select(
                                g => new Entity.ControllerAction(
                                    g.Key.controllerType.FullName.Substring(modelTarget.NameSpace.Length + 1) + "." + g.Key.action,
                                    g.Key.controllerType,
                                    g.Key.action,
                                    g.ToDictionary(v => v.httpMethod, v => v.method.method))).ToList();
                    }
                }

                return this._actions;
            }
        }

        public IList<Model> Models
        {
            get
            {
                if (this._models == null)
                {
                    foreach (NamespaceElement modelTarget in this.Configuration.Models)
                    {
                        if (string.IsNullOrWhiteSpace(modelTarget.Assembly))
                        {
                            throw new ConfigurationErrorsException("'assembly' must be specified and cannot be empty");
                        }

                        if (string.IsNullOrWhiteSpace(modelTarget.NameSpace))
                        {
                            throw new ConfigurationErrorsException("'nameSpace' must be specified and cannot be empty");
                        }

                        var assembly = Assembly.Load(modelTarget.Assembly);
                        var target = modelTarget;
                        var classes =
                            assembly.GetTypes()
                                .Where(t => t.Namespace != null && (
                                    t.Namespace.Equals(target.NameSpace, StringComparison.OrdinalIgnoreCase) ||
                                    t.Namespace.StartsWith(target.NameSpace + ".", StringComparison.OrdinalIgnoreCase)));
                        this._models = classes.Select(
                            item => new Model()
                            {
                                Class = item,
                                Name = item.FullName.Substring(modelTarget.NameSpace.Length + 1)
                            }).ToList();
                    }
                }

                return this._models;
            }
        }

        public IEnumerable<PageData> GetPageById(Guid pageId)
        {
            if (this._pagesById == null)
            {
                this._pagesById = this.Pages.GroupBy(p => p.Id).ToDictionary(g => g.Key, g => g.ToList() as IList<PageData>);
            }

            return this._pagesById.ContainsKey(pageId) ? this._pagesById[pageId] : null;
        }

        public PageData GetPageByUrl(PathString url)
        {
            if (this._pagesByUrl == null)
            {
                this._pagesByUrl = this.Pages.ToDictionary(p => p.Path, p => p);
            }

            var urlString = url.ToString().ToUpperInvariant();
            urlString = urlString[0] == '/' ? urlString : "/" + urlString;

            return this._pagesByUrl.ContainsKey(urlString) ? this._pagesByUrl[urlString] : null;
        }

        public void ResetPages()
        {
            this._pages = null;
            this._pagesById = null;
            this._pagesByUrl = null;
        }

        #endregion

        #region Methods

        public IEnumerable<PageData> GetPages()
        {
            using (var context = new CmsEntities())
            {
                var pageTypes = context.PageTypes.ToDictionary(pt => pt.PageTypeId, pt => pt);
                var items = (from page in context.Pages
                    from language in page.PageLanguages
                    select new { page, language }).ToList()
                    .Select(
                        i =>
                            new PageData(
                                i.page,
                                i.language,
                                i.page.PageTypeId.HasValue && pageTypes.ContainsKey(i.page.PageTypeId.Value) ? pageTypes[i.page.PageTypeId.Value] : null,
                                this));

                Func<PageData, string, string> parsePath = null;

                parsePath = (page, childPath) =>
                {
                    if (string.IsNullOrWhiteSpace(page.NameUrl))
                    {
                        // Dont show pages with empty url
                        return null;
                    }

                    if (page.ParentId == null)
                    {
                        var site =
                            this.Configuration.Sites.OfType<SiteElement>()
                                .FirstOrDefault(s => s.Language.Equals(page.Language, StringComparison.OrdinalIgnoreCase));
                        var path = site == null ? null : site.UrlPrefix.ToUpperInvariant() + (childPath ?? string.Empty);

                        return path == null ? null : path.StartsWith("/") ? path : "/" + path;
                    }

                    var parentPageLanguage = items.Where(i => i.Id == page.ParentId).FirstOrDefault(p => p.Language == page.Language);

                    // If the parent doesn't support the language, skip the whole url
                    return parentPageLanguage == null ? null : parsePath(parentPageLanguage, "/" + page.NameUrl.ToUpperInvariant());
                };

                var results = new List<PageData>();

                foreach (var item  in items)
                {
                    item.Path = parsePath(item, null);

                    if (item.Path != null)
                    {
                        results.Add(item);
                    }
                }

                return results;
            }
        }

        private byte[] GetResource(string resourcePath)
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                return null;
            }

            using (var reader = new BinaryReader(stream))
            {
                return reader.ReadBytes((int)stream.Length);
            }
        }

        #endregion
    }
}