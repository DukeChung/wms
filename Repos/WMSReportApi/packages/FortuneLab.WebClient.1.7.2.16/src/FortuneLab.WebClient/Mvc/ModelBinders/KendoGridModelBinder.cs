using FortuneLab.WebClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Mvc.ModelBinders
{
    public class ServerFilterRequestAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new ServerFilterRequestModelBinder();
        }
    }

    public class ServerFilterRequestModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = Activator.CreateInstance(bindingContext.ModelType);

            var valueProvider = new System.Web.Mvc.FormValueProvider(controllerContext);
            var formValues = valueProvider.GetKeysFromPrefix(string.Empty);

            var filterValue = valueProvider.GetValue("filter");
            if (filterValue != null)
            {
                var filterGotValues = filterValue.AttemptedValue;
                foreach (string item in filterGotValues.Split(new string[] { "~and~" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var filterItem = item.Split(new string[] { "~eq~", "~contains~" }, StringSplitOptions.None);
                    var pi = bindingContext.ModelType.GetProperty(filterItem[0]);
                    if (pi != null)
                    {
                        pi.SetValue(result, ReflectHelper.GetPropertyValue(pi.PropertyType, filterItem[1]));
                    }
                }
            }

            ResloveListQueryModel(bindingContext, result);

            return result;
        }



        /// <summary>
        /// 解析对象的ListQueryModel
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <param name="result"></param>
        private void ResloveListQueryModel(ModelBindingContext bindingContext, object result)
        {
            int page = 1;
            int pageSize = 20;
            string sort = null;
            string group = null;

            if (typeof(ListQueryModel).IsAssignableFrom(bindingContext.ModelType))
            {
                var listQueryModel = result as ListQueryModel;

                if (TryGetValue(bindingContext, "page", out page))
                {
                    listQueryModel.PageIndex = page < 1 ? 1 : page;
                }
                if (TryGetValue(bindingContext, "pageSize", out pageSize))
                {
                    listQueryModel.PageSize = pageSize < 1 ? 10 : pageSize;
                }
                if (TryGetValue(bindingContext, "sort", out sort))
                {
                    if (!string.IsNullOrWhiteSpace(sort))
                    {
                        var sortItem = sort.Split('-');
                        listQueryModel.SortField = sortItem[0];
                        listQueryModel.SortDirection = sortItem[1];
                    }
                }
                if (TryGetValue(bindingContext, "group", out group))
                {
                    listQueryModel.GroupProperty = string.IsNullOrWhiteSpace(group) ? null : group;
                }
            }
        }

        public bool TryGetValue<T>(ModelBindingContext bindingContext, string key, out T result)
        {
            var result2 = bindingContext.ValueProvider.GetValue(key);
            if (result2 == null)
            {
                result = default(T);
                return false;
            }
            result = (T)result2.ConvertTo(typeof(T));
            return true;
        }
    }

    public class ReflectHelper
    {
        public static T GetPropertyValue<T>(string value)
        {
            return (T)GetPropertyValue(typeof(T), value);
        }
        public static object GetPropertyValue(Type type, string value)
        {
            if (value.StartsWith("'") && value.EndsWith("'"))
                value = value.Substring(1, value.Length - 2);//移除两头的单引号

            object returnValue = null;

            switch (type.Name.ToLower())
            {
                case "guid":
                    returnValue = Guid.Parse(value);
                    break;
                case "int64":
                    returnValue = long.Parse(value);
                    break;
                case "byte":
                    returnValue = byte.Parse(value);
                    break;
                case "int32":
                case "int16":
                    returnValue = int.Parse(value);
                    break;
                case "decimal":
                    returnValue = string.IsNullOrWhiteSpace(value) ? 0M : Convert.ToDecimal(value);
                    break;
                case "double":
                    returnValue = Convert.ToDouble(value);
                    break;
                case "datetime":
                    returnValue = Convert.ToDateTime(value);
                    break;
                case "boolean":
                    if (value.Equals("on", StringComparison.OrdinalIgnoreCase) || value.Equals("off", StringComparison.OrdinalIgnoreCase))
                    {
                        returnValue = value.Equals("on", StringComparison.OrdinalIgnoreCase) ? true : false;
                    }
                    else if (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        returnValue = value.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
                    }
                    else
                    {
                        int intValue;
                        if (int.TryParse(value, out intValue))
                        {
                            returnValue = Convert.ToBoolean(intValue);
                        }
                        else
                        {
                            throw new Exception(string.Format("值{0}不是一个有效的bool值", value));
                        }
                    }
                    break;
                case "string":
                    returnValue = value;
                    break;
                case "timespan":
                    returnValue = new TimeSpan(long.Parse(value));
                    break;
                case "nullable`1":
                    returnValue = GetPropertyValue(type.GetGenericArguments()[0], value);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        returnValue = value;
                    }
                    else if (!type.IsPrimitive)
                    {
                        returnValue = JsonConvert.DeserializeObject(value, type);
                    }
                    break;
            }

            return returnValue;
        }
    }
}
