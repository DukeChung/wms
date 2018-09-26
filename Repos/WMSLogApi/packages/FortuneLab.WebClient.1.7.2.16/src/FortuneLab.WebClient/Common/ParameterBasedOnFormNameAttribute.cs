using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebAPI.Common
{
    /// <summary>
    /// If form name exists, then specified "actionParameterName" will be set to "true"
    /// </summary>
    public class ParameterBasedOnFormNameAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _name;
        private readonly string _actionParameterName;

        public ParameterBasedOnFormNameAttribute(string name, string actionParameterName)
        {
            this._name = name;
            this._actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name];
            filterContext.ActionParameters[_actionParameterName] = !string.IsNullOrEmpty(formValue);
        }
    }

    public class QueryButtonsBasedOnFormNameAttribute : FilterAttribute, IActionFilter
    {
        private readonly string[] _names;
        private readonly string[] _actionParameterNames;

        public QueryButtonsBasedOnFormNameAttribute(string[] names, string[] actionParameterNames)
        {
            this._names = names;
            this._actionParameterNames = actionParameterNames;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            for (int i = 0; i < _names.Length; i++)
            {
                filterContext.ActionParameters[_actionParameterNames[i]] =
                    filterContext.RequestContext.HttpContext.Request.Form.AllKeys.Contains(_names[i]);
            }
        }
    }

    public class QueryButtonsBasedOnFormNameValueByDashWithIntAttribute : FilterAttribute, IActionFilter
    {
        private readonly string[] _names;
        private readonly string[] _actionParameterNames;

        public QueryButtonsBasedOnFormNameValueByDashWithIntAttribute(string[] names, string[] actionParameterNames)
        {
            this._names = names;
            this._actionParameterNames = actionParameterNames;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            for (int i = 0; i < _names.Length; i++)
            {
                var key = filterContext.RequestContext.HttpContext.Request.Form.AllKeys.FirstOrDefault(x => x.StartsWith(_names[i]));
                if (key != null && key.Split('-').Length == 2)
                {
                    var strValue = key.Split('-')[1];
                    int v = 0;
                    if (int.TryParse(strValue, out v))
                    {
                        filterContext.ActionParameters[_actionParameterNames[i]] = v;
                    }
                    else
                    {
                        filterContext.ActionParameters[_actionParameterNames[i]] = null;
                    }
                }
            }
        }
    }
}