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
    public class DropdownListFilterItem : ListQueryModel
    {
        public string Text { get; set; }
        public string Name { get; set; }
    }

    public class CustomeServerFilterAttribute : CustomModelBinderAttribute
    {
        public CustomeServerFilterAttribute(Type modelBinderType = null)
        {
            ModelBinderType = modelBinderType == null ? typeof(QueryStringFilterRequestModelBinder) : modelBinderType;

        }
        public Type ModelBinderType { get; set; }

        public override IModelBinder GetBinder()
        {
            return Activator.CreateInstance(ModelBinderType) as IModelBinder;
        }
    }

    public class QueryStringFilterRequestModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = Activator.CreateInstance(bindingContext.ModelType);

            var valueProvider = new System.Web.Mvc.QueryStringValueProvider(controllerContext);
            var queryValues = valueProvider.GetKeysFromPrefix("filter[filters]");



            foreach (var filterItemKey in queryValues)
            {
                var filterItem = BuildFilterItem(filterItemKey, valueProvider);


                if (string.IsNullOrWhiteSpace(filterItem.Field) || string.IsNullOrWhiteSpace(filterItem.Value))
                {
                    continue;
                }

                var pi = bindingContext.ModelType.GetProperty(filterItem.Field);
                if (pi != null)
                {
                    pi.SetValue(result, ReflectHelper.GetPropertyValue(pi.PropertyType, filterItem.Value));
                }
            }
            return result;
        }

        private FilterItem BuildFilterItem(KeyValuePair<string, string> queryItem, QueryStringValueProvider valueProvider)
        {
            var fieldName = GetAttemptedValue($"{queryItem.Value}[field]", valueProvider);
            var fieldValue = GetAttemptedValue($"{queryItem.Value}[value]", valueProvider);
            var fieldOperator = GetAttemptedValue($"{queryItem.Value}[operator]", valueProvider);
            var fieldIgnoreCase = GetAttemptedValue($"{queryItem.Value}[ignoreCase]", valueProvider);
            return new FilterItem()
            {
                Field = fieldName,
                Value = fieldValue,
                Operator = fieldOperator,
                IgnoreCase = ReflectHelper.GetPropertyValue<bool>(fieldIgnoreCase)
            };
        }

        private static string GetAttemptedValue(string key, IValueProvider valueProvider)
        {
            var keyValue = valueProvider.GetValue(key);
            return keyValue != null ? keyValue.AttemptedValue : string.Empty;
        }
    }

    public class ServerFilterRequest
    {
        public Filter Filter { get; set; }
    }

    public class Filter
    {
        public string Logic { get; set; }
        public List<FilterItem> Filters { get; set; }
    }

    public class FilterItem
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool IgnoreCase { get; set; }
    }
}
