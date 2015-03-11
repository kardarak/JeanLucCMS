using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;

using JeanLucCMS.Entity;
using JeanLucCMS.Models;
using JeanLucCMS.Tools;

using Newtonsoft.Json;

namespace JeanLucCMS.BO
{
    internal class PageBo : IPageBo
    {
        #region Fields

        private readonly IGlobalCache _globalCache;

        #endregion

        #region Constructors

        public PageBo(IGlobalCache globalCache)
        {
            this._globalCache = globalCache;
        }

        #endregion

        #region IPageBo Members

        public Guid AddPage(Guid? parentId)
        {
            using (var context = new CmsEntities())
            {
                var item = new Page()
                {
                    PageId = Guid.NewGuid(),
                    ParentPageId = parentId,
                };

                context.Pages.Add(item);

                try
                {
                    context.SaveChanges();

                    return item.PageId;
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
                var item = context.Pages.Single(i => i.PageId == id);
                context.Pages.Remove(item);

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

        public void Update(Guid id, PagePostModel model)
        {
            using (var context = new CmsEntities())
            {
                var item = context.Pages.Single(i => i.PageId == id);

                item.PageTypeId = model.PageTypeId;
                
                foreach (var languageModel in model.Languages)
                {
                    var language = item.PageLanguages.FirstOrDefault(i => i.Language == languageModel.Language);

                    switch (languageModel.LanguagePageType)
                    {
                        case LanguagePageType.NoPage:
                            if (language != null)
                            {
                                item.PageLanguages.Remove(language);
                            }
                            break;
                        case LanguagePageType.UseOtherLanguage:
                            if (string.IsNullOrWhiteSpace(languageModel.UseLanguage))
                            {
                                if (language != null)
                                {
                                    item.PageLanguages.Remove(language);
                                }
                            }
                            else
                            {
                                if (language == null)
                                {
                                    language = new PageLanguage();
                                    language.Language = languageModel.Language;
                                    item.PageLanguages.Add(language);
                                }

                                language.NameUrl = languageModel.NameUrl;
                                language.CopyFromLanguage = languageModel.UseLanguage;
                                language.ContentJson = null;
                            }
                            break;
                        case LanguagePageType.LanguageDefined:
                            if (language == null)
                            {
                                language = new PageLanguage();
                                language.Language = languageModel.Language;
                                item.PageLanguages.Add(language);
                            }
                            language.NameUrl = languageModel.NameUrl;
                            language.CopyFromLanguage = null;
                            language.ContentJson = JsonConvert.SerializeObject(languageModel.Content);
                            
                            break;
                        default:
                            throw new InvalidDataException("Invalid page type");
                    }
                }

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