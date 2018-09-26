using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebClient.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FortuneLab
{
    public static class KendoExtensions
    {
        [Obsolete("请使用ToKendoResult替换")]
        public static DataSourceResult ToDataSourceResult<TResult>(this Page<TResult> dataList, ListQueryModel request = null, ModelStateDictionary modelState = null)
        {
            return ToDataSourceResult(dataList.Records, dataList.Paging, modelState);
        }

        [Obsolete("请使用ToKendoResult替换")]
        public static DataSourceResult ToDataSourceResult<TResult>(this IEnumerable<TResult> dataList, Paging paging = null, ModelStateDictionary modelState = null)
        {
            if (paging == null)
            {
                paging = new Paging() { PageIndex = 1, PageSize = dataList.Count(), Total = dataList.Count() };
            }

            if (typeof(IRowIndex).IsAssignableFrom(typeof(TResult)))
            {
                var i = (paging.PageIndex - 1) * paging.PageSize;
                foreach (var item in dataList)
                {
                    (item as IRowIndex).RowIndex = ++i;
                }
            }

            var result = new DataSourceResult()
            {
                Data = dataList,
                Total = paging.Total
            };

            if (modelState != null && !modelState.IsValid)
            {
                result.Errors = modelState.SerializeErrors();
            }
            return result;
        }

        /// <summary>
        /// API端返回的ApiResponse直接交给这个方法处理
        /// 错误会一并处理并在客户端展示
        /// </summary>
        /// <typeparam name="TApiResponseResult"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static JsonResult ToKendoResult<TResult>(this ApiResponse<Page<TResult>> response)
        {
            var result = new DataSourceResult();
            if (response.Success)
            {
                var records = response.ResponseResult.Records;
                var paging = response.ResponseResult.Paging;

                if (typeof(IRowIndex).IsAssignableFrom(typeof(TResult)))
                {
                    var i = (paging.PageIndex - 1) * paging.PageSize;
                    foreach (var item in records)
                    {
                        (item as IRowIndex).RowIndex = ++i;
                    }
                }

                result.Data = records;
                result.Total = paging.Total;
            }
            else
            {
                result.Errors = new JsonError(response.ApiMessage.ErrorCode, response.ApiMessage.ErrorMessage).Errors;
            }
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public static JsonResult ToKendoResult<TResult>(this ApiResponse<List<TResult>> response)
        {
            var result = new DataSourceResult();
            if (response.Success)
            {
                var records = response.ResponseResult;

                if (typeof(IRowIndex).IsAssignableFrom(typeof(TResult)))
                {
                    var i = 0;
                    foreach (var item in records)
                    {
                        (item as IRowIndex).RowIndex = ++i;
                    }
                }

                result.Data = records;
                result.Total = records.Count;
            }
            else
            {
                result.Errors = new JsonError(response.ApiMessage.ErrorCode, response.ApiMessage.ErrorMessage).Errors;
            }
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public static GridBuilder<T> ApplyGridCommonStyle<T>(this GridBuilder<T> obj)
            where T : class
        {
            return obj
                .Selectable()
                .Scrollable()
                .Sortable(sort => sort.SortMode(GridSortMode.SingleColumn))
                .Pageable(x => x.Refresh(true).PreviousNext(true).PageSizes(true).Numeric(true).Info(true))
                .Events(evt => evt.DataBinding("flab.grid.OnGridDataBinding"))
                .DataSource(dataSource => dataSource
                    .Ajax()
                    .Events(events => events.Error("flab.grid.errorHandler"))
                    );
        }

        /// <summary>
        /// Grid列头与列内容居中
        /// </summary>
        /// <typeparam name="TColumn"></typeparam>
        /// <typeparam name="TColumnBuilder"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static TColumnBuilder TextCenter<TColumn, TColumnBuilder>(this GridColumnBuilderBase<TColumn, TColumnBuilder> column)
            where TColumn : IGridColumn
            where TColumnBuilder : GridColumnBuilderBase<TColumn, TColumnBuilder>
        {
            return column.HeaderHtmlAttributes(new { @class = "text-center" }).HtmlAttributes(new { @class = "text-center" });
        }

        /// <summary>
        /// 自定义的Grid构造器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="gridName"></param>
        /// <returns></returns>
        public static GridBuilder<T> YmcGrid<T>(this WidgetFactory obj, string gridName = "") where T : class
        {
            return obj.Grid<T>()
                .Name(string.IsNullOrWhiteSpace(gridName) ? Guid.NewGuid().ToString() : gridName)
                .AutoBind(true)
                .Selectable().Scrollable().AllowCopy(true)
                .Resizable(cfg => cfg.Columns(true))
                .Sortable(sort => sort.SortMode(GridSortMode.SingleColumn))
                .Events(evt => evt.DataBinding("flab.grid.OnGridDataBinding"))
                .DataSource(dataSource => dataSource.Ajax().Events(events => events.Error("flab.grid.errorHandler")))
                .Pageable(page =>
                {
                    page.Refresh(true).PreviousNext(true).PageSizes(new List<int> { 5, 10, 20, 50, 100 }).Numeric(true).Info(true);
                    page.Messages(cfg => cfg.ItemsPerPage("条每页").Empty("无数据"));
                });
        }

        /// <summary>
        /// 应用format, 最终的格式: 2016-05-07 17:31
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTimePickerBuilder ApplyDateTimePickerFormat(this DateTimePickerBuilder obj)
        {
            var dateTimeFormat = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat;
            return obj.Format($"{dateTimeFormat.ShortDatePattern} {dateTimeFormat.ShortTimePattern}");
        }

        public static DropDownListBuilder EnumDropDownList(this WidgetFactory obj, Type enumType, string name, string enumValue = null, string optionalText = null)
        {
            var builder = obj.DropDownList().Name(name)
                .DataTextField("Text")
                .DataValueField("Value");

            var bindResult = GetEnumDescription(enumType);
            if (!string.IsNullOrWhiteSpace(optionalText))
            {
                bindResult.Insert(0, new SelectListItem() { Text = optionalText, Value = "-1" });
            }
            if (bindResult != null)
            {
                builder.BindTo(bindResult);
            }

            builder.Value(enumValue);

            return builder;
        }
        public static DropDownListBuilder EnumDropDownList<TEnum>(this WidgetFactory obj, string name, string enumValue = null, string optionalText = null)
        {
            return EnumDropDownList(obj, typeof(TEnum), name, enumValue, optionalText);
        }

        public static DropDownListBuilder EnumDropDownListFor<TModel, TProperty>(this WidgetFactory<TModel> obj, Expression<System.Func<TModel, TProperty>> expression, string optionalText = null)
        {
            return EnumDropDownList(obj, obj.HtmlHelper.ViewData.ModelMetadata.ModelType, obj.HtmlHelper.NameFor(expression).ToHtmlString(), obj.HtmlHelper.ValueFor(expression).ToHtmlString(), optionalText);
        }

        public static List<SelectListItem> GetEnumDescription(Type enumType)
        {
            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                //columnType = p.PropertyType.GetGenericArguments()[0];
                return GetEnumDescription(enumType.GetGenericArguments()[0]);
            }

            if (!enumType.IsEnum)
            {
                throw new Exception($"{enumType.FullName} is not a Enum type");
            }

            List<SelectListItem> items = new List<SelectListItem>();

            Func<Type, object, SelectListItem> GetSelectListItem = (type, enumValue) =>
            {
                string textValue;
                FieldInfo fi = enumValue.GetType().GetField(Enum.GetName(type, enumValue));
                var displayNameAttribute = fi.GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null && !string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName))
                    textValue = displayNameAttribute.DisplayName;
                else
                    textValue = Enum.GetName(type, enumValue);

                return new SelectListItem() { Text = textValue, Value = enumValue.ToString() };
            };

            Func<object, string> GetDisplayName = o =>
            {
                var result = null as string;
                var display = o.GetType()
                               .GetMember(o.ToString()).First()
                               .GetCustomAttributes(false)
                               .OfType<DisplayNameAttribute>()
                               .LastOrDefault();
                if (display != null)
                {
                    result = display.DisplayName;
                }

                return result ?? o.ToString();
            };

            foreach (var item in Enum.GetValues(enumType))
            {
                items.Add(GetSelectListItem(enumType, item));
            }
            return items;
        }
    }
}
