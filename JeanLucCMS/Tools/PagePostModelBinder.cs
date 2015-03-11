using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

using JeanLucCMS.Models;

namespace JeanLucCMS.Tools
{
    /// <summary>
    /// Based on: http://www.codeproject.com/Articles/701182/A-Custom-Model-Binder-for-Passing-Complex-Objects
    /// </summary>
    internal class PagePostModelBinder : IModelBinder
    {
        #region Champs

        private const string RexChechNumeric = @"^\d+$";
        private const string RexBrackets = @"\[\d*\]|\.";
        private const string RexSearchBracket = @"\[([^}])\]\.?|\."; // @"\[([^}])\]";
        private readonly IGlobalCache _globalCache;
        private readonly HttpParameterDescriptor _param;

        //Define original source data list

        //Set default maximum resursion limit
        private readonly int _maxRecursionLimit = 100;
        private List<KeyValuePair<string, string>> _kvps;
        private int _recursionCount = 0;
        private Type _contentType;

        #endregion

        #region Constructeurs

        public PagePostModelBinder(IGlobalCache globalCache, HttpParameterDescriptor param)
        {
            this._globalCache = globalCache;
            this._param = param;
        }

        #endregion

        #region Membres IModelBinder

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(PagePostModel))
            {
                return false;
            }

            if (!actionContext.Request.Content.IsFormData())
            {
                return false;
            }

            var bodyString = actionContext.Request.Content.ReadAsStringAsync().Result;
            try
            {
                this._kvps = this.ConvertToKvps(bodyString);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                return false;
            }

            var pageTypeIdString = this._kvps.FirstOrDefault(kvp => kvp.Key == "PageTypeId").Value;

            Guid pageTypeId;
            if (Guid.TryParse(pageTypeIdString, out pageTypeId))
            {
                var pageType = new PageTypeModel(this._globalCache, pageTypeId).Item;
                var modelType = pageType == null ? null : this._globalCache.Models.FirstOrDefault(m => m.Name == pageType.ModelType);
                this._contentType = modelType == null ? null : modelType.Class;
            }

            var obj = Activator.CreateInstance(bindingContext.ModelType);

            bindingContext.Model = obj;
            this.SetPropertyValues(obj);

            return true;
        }

        #endregion

        #region Méthodes

        public void SetPropertyValues(object obj, object parentObj = null, PropertyInfo parentProp = null)
        {
            //Recursively set PropertyInfo array for object hierarchy
            var props = obj.GetType().GetProperties();

            //Set KV Work List for real iteration process so that kvps is not in iteration and
            //its items from kvps can be removed after each iteration

            foreach (var prop in props)
            {
                //Refresh KV Work list from refreshed base kvps list after processing each property
                var kvpsWork = new List<KeyValuePair<string, string>>(this._kvps);

                //Check and process property encompassing complex object recursively 
                if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String")
                {
                    this.RecurseNestedObj(obj, prop);
                }
                else
                {
                    foreach (var item in kvpsWork)
                    {
                        //Ignore any bracket in a name key 
                        var key = item.Key;
                        var keyParts = Regex.Split(key, RexBrackets);
                        if (keyParts.Length > 1) key = keyParts[keyParts.Length - 1];
                        if (key == prop.Name)
                        {
                            //Populate KeyValueWork and pass it for adding property to object
                            var kvw = new KeyValueWork()
                            {
                                Key = item.Key,
                                Value = item.Value,
                                SourceKvp = item
                            };
                            this.AddSingleProperty(obj, prop, kvw);
                            break;
                        }
                    }
                }
            }
            //Add property of this object to parent object 
            if (parentObj != null && parentProp != null)
            {
                parentProp.SetValue(parentObj, obj, null);
            }
        }

        public void SetPropertyValuesForList(
            object obj,
            object parentObj = null,
            PropertyInfo parentProp = null,
            string pParentName = "",
            string pParentObjIndex = "")
        {
            //Get props for type of object item in collection
            var props = obj.GetType().GetProperties();
            //KV Work For each object item in collection
            var kvwsGroup = new List<KeyValueWork>();
            //KV Work for collection
            var kvwsGroups = new List<List<KeyValueWork>>();

            var isGroupAdded = false;
            var lastIndex = "";

            foreach (var item in this._kvps)
            {
                //Passed parentObj and parentPropName are for List, whereas obj is instance of type for List
                if (parentProp != null && item.Key.Contains(parentProp.Name))
                {
                    //Get data only from parent-parent for linked child KV Work
                    Regex regex;
                    Match match;
                    if (pParentName != "" & pParentObjIndex != "")
                    {
                        regex = new Regex(pParentName + RexSearchBracket);
                        match = regex.Match(item.Key);
                        if (match.Groups[1].Value != pParentObjIndex) break;
                    }
                    //Get parts from current KV Work
                    regex = new Regex(parentProp.Name + RexSearchBracket);
                    match = regex.Match(item.Key);
                    var brackets = match.Value.Replace(parentProp.Name, "");
                    var objIdx = match.Groups[1].Value;

                    //Point to start next idx and save last kvwsGroup data to kvwsGroups
                    if (lastIndex != "" && objIdx != lastIndex)
                    {
                        kvwsGroups.Add(kvwsGroup);
                        isGroupAdded = true;
                        kvwsGroup = new List<KeyValueWork>();
                    }
                    //Get parts array from Key
                    var keyParts = item.Key.Split(new string[] { brackets }, StringSplitOptions.RemoveEmptyEntries);
                    //Populate KV Work
                    var kvw = new KeyValueWork()
                    {
                        ObjIndex = objIdx,
                        ParentName = parentProp.Name,
                        //Get last part from prefixed name
                        Key = keyParts[keyParts.Length - 1],
                        Value = item.Value,
                        SourceKvp = item
                    };
                    //add KV Work to kvwsGroup list
                    kvwsGroup.Add(kvw);
                    lastIndex = objIdx;
                    isGroupAdded = false;
                }
            }
            //Handle the last kvwsgroup item if not added to final kvwsGroups List.
            if (kvwsGroup.Count > 0 && isGroupAdded == false) kvwsGroups.Add(kvwsGroup);

            //Initiate List or Array
            IList listObj = null;
            Array arrayObj = null;
            if (parentProp.PropertyType.IsGenericType || parentProp.PropertyType.BaseType.IsGenericType)
            {
                listObj = (IList)Activator.CreateInstance(parentProp.PropertyType);
            }
            else if (parentProp.PropertyType.IsArray)
            {
                arrayObj = Array.CreateInstance(parentProp.PropertyType.GetElementType(), kvwsGroups.Count);
            }

            var idx = 0;
            foreach (var group in kvwsGroups)
            {
                //Initiate object with type of collection item
                var tempObj = Activator.CreateInstance(obj.GetType());
                foreach (var prop in props)
                {
                    //Check and process nested objects in collection recursively
                    //Pass ObjIndex for child KV Work items only for this parent object
                    if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String")
                    {
                        this.RecurseNestedObj(tempObj, prop, pParentName: group[0].ParentName, pParentObjIndex: group[0].ObjIndex);
                    }
                    else
                    {
                        //Assign terminal property to object    
                        foreach (var item in group)
                        {
                            if (item.Key == prop.Name)
                            {
                                this.AddSingleProperty(tempObj, prop, item);
                                break;
                            }
                        }
                    }
                }
                //Add populated object to List or Array                    
                if (listObj != null)
                {
                    listObj.Add(tempObj);
                }
                else if (arrayObj != null)
                {
                    arrayObj.SetValue(tempObj, idx);
                    idx++;
                }
            }
            //Add property for List or Array into parent object 
            if (listObj != null)
            {
                parentProp.SetValue(parentObj, listObj, null);
            }
            else if (arrayObj != null)
            {
                parentProp.SetValue(parentObj, arrayObj, null);
            }
        }

        private void AddSingleProperty(object obj, PropertyInfo prop, KeyValueWork item)
        {
            if (prop.PropertyType.IsEnum)
            {
                var enumValues = prop.PropertyType.GetEnumValues();
                object enumValue = null;
                var isFound = false;

                //Try to match enum item name first
                for (var i = 0; i < enumValues.Length; i++)
                {
                    if (item.Value.ToLower() == enumValues.GetValue(i).ToString().ToLower())
                    {
                        enumValue = enumValues.GetValue(i);
                        isFound = true;
                        break;
                    }
                }
                //Try to match enum default underlying int value if not matched with enum item name
                if (!isFound)
                {
                    for (var i = 0; i < enumValues.Length; i++)
                    {
                        if (item.Value == i.ToString())
                        {
                            enumValue = i;
                            break;
                        }
                    }
                }
                prop.SetValue(obj, enumValue, null);
            }
            else if (prop.PropertyType == typeof(Guid?))
            {
                Guid guid;

                if (Guid.TryParse(item.Value, out guid))
                {
                    prop.SetValue(obj, guid);
                }
            }
            else
            {
                //Set value for non-enum terminal property 
                prop.SetValue(obj, Convert.ChangeType(item.Value, prop.PropertyType), null);
            }
            this._kvps.Remove(item.SourceKvp);
        }

        private List<KeyValuePair<string, string>> ConvertToKvps(string sourceString)
        {
            var kvpList = new List<KeyValuePair<string, string>>();
            if (sourceString.StartsWith("?")) sourceString = sourceString.Substring(1);
            var elements = sourceString.Split('=', '&');
            for (var i = 0; i < elements.Length; i += 2)
            {
                var kvp = new KeyValuePair<string, string>
                    (
                    WebUtility.UrlDecode(elements[i]),
                    WebUtility.UrlDecode(elements[i + 1])
                    );
                kvpList.Add(kvp);
            }
            return kvpList;
        }

        private void RecurseNestedObj(object obj, PropertyInfo prop, string pParentName = "", string pParentObjIndex = "")
        {
            //Check recursion limit
            if (this._recursionCount > this._maxRecursionLimit)
            {
                throw new Exception(string.Format("Exceed maximum recursion limit {0}", this._maxRecursionLimit));
            }
            this._recursionCount++;

            //Valicate collection types
            if (prop.PropertyType.IsGenericType || (prop.PropertyType.BaseType != null && prop.PropertyType.BaseType.IsGenericType))
            {
                if ((prop.PropertyType.IsGenericType && prop.PropertyType.Name != "List`1")
                    || (prop.PropertyType.BaseType != null && prop.PropertyType.BaseType.IsGenericType && prop.PropertyType.BaseType.Name != "List`1"))
                {
                    throw new Exception("Only support nested Generic List collection");
                }
                if (prop.PropertyType.GenericTypeArguments.Length > 1
                    || (prop.PropertyType.BaseType != null && prop.PropertyType.BaseType.GenericTypeArguments.Length > 1))
                {
                    throw new Exception("Only support nested Generic List collection with one argument");
                }
            }

            object childObj = null;
            if (prop.PropertyType.IsGenericType || (prop.PropertyType.BaseType != null && prop.PropertyType.BaseType.IsGenericType)
                || prop.PropertyType.IsArray)
            {
                //Dynamically create instances for nested collection items
                if (prop.PropertyType.IsGenericType)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
                }
                else if (!prop.PropertyType.IsGenericType && (prop.PropertyType.BaseType != null && prop.PropertyType.BaseType.IsGenericType))
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.BaseType.GenericTypeArguments[0]);
                }
                else if (prop.PropertyType.IsArray)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GetElementType());
                }

                //Call to process collection
                this.SetPropertyValuesForList(
                    childObj,
                    parentObj: obj,
                    parentProp: prop,
                    pParentName: pParentName,
                    pParentObjIndex: pParentObjIndex);
            }
            else
            {
                //Dynamically create instances for nested object and call to process it
                if (obj is PageLanguageModel && prop.Name == "Content")
                {
                    childObj = Activator.CreateInstance(this._contentType);
                }
                else
                {
                    childObj = Activator.CreateInstance(prop.PropertyType);
                }
                this.SetPropertyValues(childObj, parentObj: obj, parentProp: prop);
            }
        }

        #endregion

        #region Types imbriqués

        private sealed class KeyValueWork
        {
            #region Propriétés

            internal string Key { get; set; }
            internal string ObjIndex { get; set; }
            internal string ParentName { get; set; }
            internal KeyValuePair<string, string> SourceKvp { get; set; }
            internal string Value { get; set; }

            #endregion
        }

        #endregion
    }
}