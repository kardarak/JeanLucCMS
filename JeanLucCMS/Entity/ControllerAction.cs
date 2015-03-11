using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace JeanLucCMS.Entity
{
    public class ControllerAction
    {
        #region Constructors

        public ControllerAction(string fullName, Type controllerType, string actionName, IDictionary<HttpMethod, MethodInfo> methods)
        {
            this.FullName = fullName;
            this.ControllerType = controllerType;
            this.ActionName = actionName;
            this.Methods = methods;
        }

        #endregion

        #region Properties

        public string ActionName { get; private set; }

        public Type ControllerType { get; private set; }

        public string FullName { get; private set; }

        public IDictionary<HttpMethod, MethodInfo> Methods { get; private set; }

        #endregion
    }
}