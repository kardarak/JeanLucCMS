using System;
using System.Collections.Generic;
using System.Linq;

using JeanLucCMS.Entity;
using JeanLucCMS.Tools;

namespace JeanLucCMS.Models
{
    public class PageTypeModel
    {
        #region Constructors

        internal PageTypeModel(IGlobalCache cache)
            : this(cache, Guid.Empty)
        {
        }

        internal PageTypeModel(IGlobalCache cache, Guid id)
        {
            this.Actions = cache.ControllerActions;
            this.Models = cache.Models;

            using (var context = new CmsEntities())
            {
                this.PageTypes = this.GetPageTypes(context).ToList();

                if (id != Guid.Empty)
                {
                    var item = context.PageTypes.FirstOrDefault(i => i.PageTypeId == id);

                    this.Item = item == null
                        ? null
                        : new PageTypeItemModel()
                        {
                            Id = id,
                            Name = item.Name,
                            IsFolder = item.IsFolder,
                            ActionFullName =
                                cache.ControllerActions.Where(a => a.ControllerType.FullName == item.ControllerType && a.ActionName == item.ActionName)
                                    .Select(a => a.FullName)
                                    .FirstOrDefault(),
                            ModelType = item.ModelType
                        };
                }
            }
        }

        #endregion

        #region Properties

        public IList<Entity.ControllerAction> Actions { get; private set; }

        public PageTypeItemModel Item { get; set; }
        public IList<Entity.Model> Models { get; private set; }
        public IList<PageTypeItemModel> PageTypes { get; private set; }

        #endregion

        #region Methods

        private IEnumerable<PageTypeItemModel> GetPageTypes(CmsEntities context)
        {
            var items = context.PageTypes.ToDictionary(
                pt => pt.PageTypeId,
                pt => new PageTypeItemModel()
                {
                    Id = pt.PageTypeId,
                    Name = pt.Name,
                    IsFolder = pt.IsFolder,
                    ParentId = pt.ParentPageTypeId
                });

            foreach (var item in items.Values.Where(i => i.ParentId.HasValue))
            {
                items[item.ParentId.Value].Childrens.Add(item);
            }

            return items.Values.Where(i => i.ParentId == null);
        }

        #endregion
    }
}