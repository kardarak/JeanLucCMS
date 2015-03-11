using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace JeanLucCMS.Tools
{
    internal class AdminContentMiddleware : OwinMiddleware
    {
        #region Fields

        private readonly GlobalCache _globalCache;

        private readonly Dictionary<string, string> _mimeTypesByExtensions = new Dictionary<string, string>()
        {
            { "CSS", "text/css" },
            { "JS", "application/javascript" },
            { "EOT", "font/opentype" },
            { "SVG", "font/opentype" },
            { "TTF", "font/opentype" },
            { "WOFF", "font/opentype" },
            { "GIF", "image/gif" },
            { "PNG", "image/png" },
            { "JPG", "image/jpeg" },
            { "JPEG", "image/jpeg" }
        };

        #endregion

        #region Constructors

        public AdminContentMiddleware(OwinMiddleware next, GlobalCache globalCache)
            : base(next)
        {
            this._globalCache = globalCache;
        }

        #endregion

        #region Methods

        public override async Task Invoke(IOwinContext context)
        {
            var path = context.Request.Path.ToString();
            var adminPath = "/" + this._globalCache.Configuration.AdminPath;

            if (path.StartsWith(adminPath, StringComparison.OrdinalIgnoreCase))
            {
                var pathParts = path.Substring(adminPath.Length)
                    .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathParts.Length > 0 && pathParts[0].Equals("content", StringComparison.OrdinalIgnoreCase))
                {
                    var key = string.Join(".", pathParts);

                    var content =
                        this._globalCache.AdminContent.Where(d => d.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                            .Select(d => d.Value)
                            .FirstOrDefault();

                    if (content != null)
                    {
                        var ext = key.ToUpperInvariant().Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                        using (var writer = new BinaryWriter(context.Response.Body))
                        {
                            if (ext != null && this._mimeTypesByExtensions.ContainsKey(ext))
                            {
                                context.Response.Headers["Content-Type"] = this._mimeTypesByExtensions[ext];
                            }

                            writer.Write(content);
                            return;
                        }
                    }
                }
            }

            await this.Next.Invoke(context);
        }

        #endregion
    }
}