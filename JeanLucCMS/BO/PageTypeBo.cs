using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;

using JeanLucCMS.Entity;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

namespace JeanLucCMS.BO
{
    internal class PageTypeBo : IPageTypeBo
    {
        #region Fields

        private readonly IGlobalCache _globalCache;

        #endregion

        #region Constructors

        public PageTypeBo(IGlobalCache globalCache)
        {
            this._globalCache = globalCache;
        }

        #endregion

        #region IPageTypeBo Members

        public Guid AddFolder(Guid? parentId)
        {
            using (var context = new CmsEntities())
            {
                var item = new PageType()
                {
                    PageTypeId = Guid.NewGuid(),
                    ParentPageTypeId = parentId,
                    IsFolder = true,
                    Name = "Folder"
                };

                context.PageTypes.Add(item);

                try
                {
                    context.SaveChanges();

                    return item.PageTypeId;
                }
                catch (DbEntityValidationException ex)
                {
                    var errors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors);

                    var message = "Validation errors:\n";
                    message += string.Join("\n", errors.Select(e => e.PropertyName + ": " + e.ErrorMessage));

                    throw new ValidationException(message);
                }
            }
        }

        public Guid AddPageType(Guid parentId)
        {
            using (var context = new CmsEntities())
            {
                var item = new PageType()
                {
                    PageTypeId = Guid.NewGuid(),
                    ParentPageTypeId = parentId,
                    IsFolder = false,
                    Name = "Page type"
                };

                context.PageTypes.Add(item);

                try
                {
                    context.SaveChanges();

                    return item.PageTypeId;
                }
                catch (DbEntityValidationException ex)
                {
                    var errors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors);

                    var message = "Validation errors:\n";
                    message += string.Join("\n", errors.Select(e => e.PropertyName + ": " + e.ErrorMessage));

                    throw new ValidationException(message);
                }
            }
        }

        public void Delete(Guid id)
        {
            using (var context = new CmsEntities())
            {
                var item = context.PageTypes.Single(i => i.PageTypeId == id);
                context.PageTypes.Remove(item);

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors);

                    var message = "Validation errors:\n";
                    message += string.Join("\n", errors.Select(e => e.PropertyName + ": " + e.ErrorMessage));

                    throw new ValidationException(message);
                }
            }
        }

        public void Update(Guid id, PageTypePostModel model)
        {
            using (var context = new CmsEntities())
            {
                var item = context.PageTypes.Single(i => i.PageTypeId == id);
                var controllerAction = this._globalCache.ControllerActions.FirstOrDefault(a => a.FullName == model.ActionFullName);

                item.Name = model.Name;
                item.ModelType = model.ModelType;
                item.ControllerType = controllerAction == null ? null : controllerAction.ControllerType.FullName;
                item.ActionName = controllerAction == null ? null : controllerAction.ActionName;

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors);

                    var message = "Validation errors:\n";
                    message += string.Join("\n", errors.Select(e => e.PropertyName + ": " + e.ErrorMessage));

                    throw new ValidationException(message);
                }
            }
        }

        #endregion
    }
}