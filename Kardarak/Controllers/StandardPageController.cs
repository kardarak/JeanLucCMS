using System;
using System.Web.Http;

using JeanLucCMS;

using Kardarak.Models;
using Kardarak.Models.Cms;

namespace Kardarak.Controllers
{
    public class StandardPageController : BaseController
    {
        #region Methods

        [ActionName("Show")]
        [HttpGet]
        public IHttpActionResult Show()
        {
            return this.View("StandardPage\\Index", this.GetContent<StandardPageModel>());
        }

        [ActionName("Test")]
        [HttpGet]
        public IHttpActionResult Test()
        {
            return this.View("StandardPage\\Test", this.GetContent<StandardPageModel>());
        }

        [ActionName("Test")]
        [HttpPost]
        public IHttpActionResult Test(TestPostModel model)
        {
            var pageModel = this.GetContent<StandardPageModel>();

            pageModel.TextTest = model.TestA;

            return this.View("StandardPage\\Test", pageModel);
        }

        #endregion
    }
}