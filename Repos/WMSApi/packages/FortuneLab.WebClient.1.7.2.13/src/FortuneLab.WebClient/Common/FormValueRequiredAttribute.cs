using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebAPI.Common
{
    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly string[] _submitButtonNames;
        private readonly FormValueRequirement _requirement;

        public FormValueRequiredAttribute(params string[] submitButtonNames) :
            this(FormValueRequirement.Equal, submitButtonNames)
        {
        }

        public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames)
        {
            //at least one submit button should be found
            this._submitButtonNames = submitButtonNames;
            this._requirement = requirement;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (string buttonName in _submitButtonNames)
            {
                try
                {
                    string value = "";
                    switch (this._requirement)
                    {
                        case FormValueRequirement.Equal:
                            //do not iterate because "Invalid request" exception can be thrown
                            if (controllerContext.HttpContext.Request.Form.AllKeys.Contains(buttonName))
                            {
                                return true;
                            }
                            break;
                        case FormValueRequirement.StartsWith:
                            if (controllerContext.HttpContext.Request.Form.AllKeys.FirstOrDefault(x => x.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase)) != null)
                            {
                                return true;
                            }
                            break;
                    }
                    if (!String.IsNullOrEmpty(value))
                        return true;
                }
                catch 
                {
                    //try-catch to ensure that 
                    //Debug.WriteLine(exc.Message);
                }
            }
            return false;
        }
    }

    public enum FormValueRequirement
    {
        Equal,
        StartsWith
    }
}