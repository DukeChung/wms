﻿/*
 ASP.NET MvcPager control
 Copyright:2009-2010 Webdiyer (http://en.webdiyer.com)
 Source code released under Ms-PL license
 */
using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

namespace Webdiyer.WebControls.Mvc
{
    internal class PagerBuilder
    {
        private readonly HtmlHelper _html;
        private readonly AjaxHelper _ajax;
        private readonly string _actionName;
        private readonly string _controllerName;
        private readonly int _totalPageCount = 1;
        private readonly int _pageIndex;
        private readonly PagerOptions _pagerOptions;
        private readonly RouteValueDictionary _routeValues;
        private readonly string _routeName;
        private readonly int _startPageIndex = 1;
        private readonly int _endPageIndex = 1;
        private readonly bool _msAjaxPaging;
        private readonly AjaxOptions _ajaxOptions;
        private IDictionary<string, object> _htmlAttributes;
        private const string CopyrightText = "\r\n<!--ASP.NET MvcPager 1.3 for ASP.NET MVC 1.0 Copyright © 2009-2010 Webdiyer (http://en.webdiyer.com)-->\r\n";
        private const string JqueryScriptCheckItemKey = "_MvcPager_CheckjQueryScript";
        private const string ScriptPageIndexName = "*_MvcPager_PageIndex_*";
        private const string JqCheckScript = "if(typeof(jQuery)==\"undefined\"){alert(\"jQuery library not detected, please make sure it loaded prior to MvcPager!\");}";
        private const string GoToPageScript = "function _MvcPager_GoToPage(_pib,_mp){var pageIndex;if(_pib.tagName==\"SELECT\"){pageIndex=_pib.options[_pib.selectedIndex].value;}else{pageIndex=_pib.value;var r=new RegExp(\"^\\\\s*(\\\\d+)\\\\s*$\");if(!r.test(pageIndex)){alert(\"%InvalidPageIndexErrorMessage%\");return;}else if(RegExp.$1<1||RegExp.$1>_mp){alert(\"%PageIndexOutOfRangeErrorMessage%\");return;}}var _hl=document.getElementById(_pib.id+'link').childNodes[0];var _lh=_hl.href;_hl.href=_lh.replace('" + ScriptPageIndexName + "',pageIndex);%ClickHandler%;_hl.href=_lh;}";
        private const string KeyDownScript = "function _MvcPager_Keydown(e){var _kc,_pib;if(window.event){_kc=e.keyCode;_pib=e.srcElement;}else if(e.which){_kc=e.which;_pib=e.target;}var validKey=(_kc==8||_kc==46||_kc==37||_kc==39||(_kc>=48&&_kc<=57)||(_kc>=96&&_kc<=105));if(!validKey){if(_kc==13){ _MvcPager_GoToPage(_pib,%TotalPageCount%);}if(e.preventDefault){e.preventDefault();}else{event.returnValue=false;}}}";

        /// <summary>
        /// used when PagedList is null
        /// </summary>
        internal PagerBuilder(HtmlHelper html, AjaxHelper ajax, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            _html = html;
            _ajax = ajax;
            _pagerOptions = pagerOptions;
            _htmlAttributes = htmlAttributes;
        }

        internal PagerBuilder(HtmlHelper html, AjaxHelper ajax, string actionName, string controllerName,
            int totalPageCount, int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            _msAjaxPaging = (ajax != null);
            if (String.IsNullOrEmpty(actionName))
            {
                if (routeValues != null && routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                else if (ajax != null)
                    actionName = (string)ajax.ViewContext.RouteData.Values["action"];
                else
                    actionName = (string)html.ViewContext.RouteData.Values["action"];
            }
            if (String.IsNullOrEmpty(controllerName))
            {
                if (routeValues != null && routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
                else if (ajax != null)
                    controllerName = (string)ajax.ViewContext.RouteData.Values["controller"];
                else
                    controllerName = (string)html.ViewContext.RouteData.Values["controller"];
            }
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();

            _html = html;
            _ajax = ajax;
            _actionName = actionName;
            _controllerName = controllerName;
            _totalPageCount = totalPageCount;
            _pageIndex = pageIndex;
            _pagerOptions = pagerOptions;
            _routeName = routeName;
            _routeValues = routeValues;
            _ajaxOptions = ajaxOptions;
            _htmlAttributes = htmlAttributes;

            // start page index
            _startPageIndex = pageIndex - (pagerOptions.NumericPagerItemCount / 2);
            if (_startPageIndex + pagerOptions.NumericPagerItemCount > _totalPageCount)
                _startPageIndex = _totalPageCount + 1 - pagerOptions.NumericPagerItemCount;
            if (_startPageIndex < 1)
                _startPageIndex = 1;

            // end page index
            _endPageIndex = _startPageIndex + _pagerOptions.NumericPagerItemCount - 1;
            if (_endPageIndex > _totalPageCount)
                _endPageIndex = _totalPageCount;
        }
        //non Ajax pager builder
        internal PagerBuilder(HtmlHelper helper, string actionName, string controllerName, int totalPageCount,
            int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
            : this(helper, null, actionName, controllerName,
                totalPageCount, pageIndex, pagerOptions, routeName, routeValues, null, htmlAttributes)
        { }
        //Microsoft Ajax pager builder
        internal PagerBuilder(AjaxHelper helper, string actionName, string controllerName, int totalPageCount,
            int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
            : this(null, helper, actionName,
                controllerName, totalPageCount, pageIndex, pagerOptions, routeName, routeValues, ajaxOptions, htmlAttributes)
        { }
        //jQuery Ajax pager builder
        internal PagerBuilder(HtmlHelper helper, string actionName, string controllerName, int totalPageCount,
            int pageIndex, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
            : this(helper, null, actionName,
                controllerName, totalPageCount, pageIndex, pagerOptions, routeName, routeValues, ajaxOptions, htmlAttributes)
        { }

        private void AddPrevious(ICollection<PagerItem> results)
        {
            var item = new PagerItem(_pagerOptions.PrevPageText, _pageIndex - 1, _pageIndex == 1, PagerItemType.PrevPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }

        private void AddFirst(ICollection<PagerItem> results)
        {
            PagerItem item = new PagerItem(_pagerOptions.FirstPageText, 1, _pageIndex == 1, PagerItemType.FirstPage);

            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }

        private void AddMoreBefore(ICollection<PagerItem> results)
        {
            if (_startPageIndex > 1 && _pagerOptions.ShowMorePagerItems)
            {
                var index = _startPageIndex - 1;
                if (index < 1) index = 1;
                PagerItem item = new PagerItem(_pagerOptions.MorePageText, index, false, PagerItemType.MorePage);
                results.Add(item);
            }
        }

        private void AddPageNumbers(ICollection<PagerItem> results)
        {
            for (var pageIndex = _startPageIndex; pageIndex <= _endPageIndex; pageIndex++)
            {
                var text = pageIndex.ToString();
                if (pageIndex == _pageIndex && !string.IsNullOrEmpty(_pagerOptions.CurrentPageNumberFormatString))
                    text = String.Format(_pagerOptions.CurrentPageNumberFormatString, text);
                else if (!string.IsNullOrEmpty(_pagerOptions.PageNumberFormatString))
                    text = String.Format(_pagerOptions.PageNumberFormatString, text);
                var item = new PagerItem(text, pageIndex, false, PagerItemType.NumericPage);
                results.Add(item);
            }
        }

        private void AddMoreAfter(ICollection<PagerItem> results)
        {
            if (_endPageIndex < _totalPageCount)
            {
                var index = _startPageIndex + _pagerOptions.NumericPagerItemCount;
                if (index > _totalPageCount) { index = _totalPageCount; }
                var item = new PagerItem(_pagerOptions.MorePageText, index, false, PagerItemType.MorePage);
                results.Add(item);
            }
        }

        private void AddNext(ICollection<PagerItem> results)
        {
            var item = new PagerItem(_pagerOptions.NextPageText, _pageIndex + 1, _pageIndex >= _totalPageCount, PagerItemType.NextPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }

        private void AddLast(ICollection<PagerItem> results)
        {
            var item = new PagerItem(_pagerOptions.LastPageText, _totalPageCount, _pageIndex >= _totalPageCount, PagerItemType.LastPage);
            if (!item.Disabled || (item.Disabled && _pagerOptions.ShowDisabledPagerItems))
                results.Add(item);
        }

        /// <summary>
        /// generate paging url
        /// </summary>
        /// <param name="pageIndex">page index to generate navigate url</param>
        /// <returns>navigated url for pager item</returns>
        private string GenerateUrl(int pageIndex)
        {
            //return null if  page index larger than total page count or page index is current page index
            if (pageIndex > _totalPageCount || pageIndex == _pageIndex)
                return null;

            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues.Merge(_routeValues);

            // set route value of page index parameter name in url,pageIndex==0 is a special case
            if (pageIndex == 0)
                routeValues[_pagerOptions.PageIndexParameterName] = ScriptPageIndexName;
            else
                routeValues[_pagerOptions.PageIndexParameterName] = pageIndex;

            var rq = _html.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in rq.Keys)
            {
                if (!string.Equals(key, _pagerOptions.PageIndexParameterName, StringComparison.OrdinalIgnoreCase))
                    routeValues[key] = rq[key];
            }
            //// Add action
            //routeValues["action"] = _actionName;

            //// Add controller
            //routeValues["controller"] = _controllerName;

            // Return link
            var urlHelper = new UrlHelper(_html.ViewContext.RequestContext);
            if (!string.IsNullOrEmpty(_routeName))
            {
                return urlHelper.RouteUrl(_routeName, routeValues);
            }
            else
            {
               // return urlHelper.RouteUrl(RouteValuesHelpers.MergeRouteValues(_actionName, _controllerName, _routeValues, _html.ViewContext.RouteData.Values, true));
                return urlHelper.RouteUrl(RouteValuesHelpers.MergeRouteValues(_actionName, _controllerName, _html.ViewContext.RouteData.Values, routeValues, true));
            }
        }

        /// <summary>
        /// render paging control
        /// </summary>
        /// <returns></returns>
        internal string RenderPager()
        {
            //return null if total page count less than or equal to 1
            if (_totalPageCount <= 1 && _pagerOptions.AutoHide)
                return CopyrightText;
            //Display error message if pageIndex out of range
            if ((_pageIndex > _totalPageCount && _totalPageCount > 0) || _pageIndex < 1)
            {
                return
                    string.Format("{0}<div style=\"color:red;font-weight:bold\">{1}</div>{0}",
                                         CopyrightText, _pagerOptions.PageIndexOutOfRangeErrorMessage);
            }


            var pagerItems = new List<PagerItem>();
            //First page
            if (_pagerOptions.ShowFirstLast)
                AddFirst(pagerItems);

            // Prev page
            if (_pagerOptions.ShowPrevNext)
                AddPrevious(pagerItems);


            if (_pagerOptions.ShowNumericPagerItems)
            {
                if (_pagerOptions.AlwaysShowFirstLastPageNumber && _startPageIndex > 1)
                    pagerItems.Add(new PagerItem("1", 1, false, PagerItemType.NumericPage));

                // more page before numeric page buttons
                if (_pagerOptions.ShowMorePagerItems)
                    AddMoreBefore(pagerItems);

                // numeric page
                AddPageNumbers(pagerItems);

                // more page after numeric page buttons
                if (_pagerOptions.ShowMorePagerItems)
                    AddMoreAfter(pagerItems);

                if (_pagerOptions.AlwaysShowFirstLastPageNumber && _endPageIndex < _totalPageCount)
                    pagerItems.Add(new PagerItem(_totalPageCount.ToString(), _totalPageCount, false,
                                                 PagerItemType.NumericPage));
            }

            // Next page
            if (_pagerOptions.ShowPrevNext)
                AddNext(pagerItems);

            //Last page
            if (_pagerOptions.ShowFirstLast)
                AddLast(pagerItems);

            var sb = new StringBuilder();
            if (_msAjaxPaging)
            {
                foreach (PagerItem item in pagerItems)
                {
                    sb.Append(GenerateMsAjaxPagerElement(item));
                }
            }
            else if (_pagerOptions.UseJqueryAjax)
            {
                foreach (PagerItem item in pagerItems)
                {
                    sb.Append(GenerateJqAjaxPagerElement(item));
                }
            }
            else
            {
                foreach (PagerItem item in pagerItems)
                {
                    sb.Append(GeneratePagerElement(item));
                }
            }
            var tb = new TagBuilder(_pagerOptions.ContainerTagName);
            if (!string.IsNullOrEmpty(_pagerOptions.Id))
                tb.GenerateId(_pagerOptions.Id);
            if (!string.IsNullOrEmpty(_pagerOptions.CssClass))
                tb.AddCssClass(_pagerOptions.CssClass);
            if (!string.IsNullOrEmpty(_pagerOptions.HorizontalAlign))
            {
                string strAlign = "text-align:" + _pagerOptions.HorizontalAlign.ToLower();
                if (_htmlAttributes == null)
                    _htmlAttributes = new RouteValueDictionary { { "style", strAlign } };
                else
                {
                    if (_htmlAttributes.Keys.Contains("style"))
                        _htmlAttributes["style"] += ";" + strAlign;
                }
            }
            tb.MergeAttributes(_htmlAttributes, true);
            string pagerScript = string.Empty;
            if (_pagerOptions.UseJqueryAjax &&
                (string)_html.ViewContext.HttpContext.Items[JqueryScriptCheckItemKey] != "1")
            {
                pagerScript = JqCheckScript;
                _html.ViewContext.HttpContext.Items[JqueryScriptCheckItemKey] = "1";
            }
            if (_pagerOptions.ShowPageIndexBox)
            {
                sb.Append(BuildGoToPageSection(ref pagerScript));
            }
            else
                sb.Length -= _pagerOptions.SeparatorHtml.Length;
            tb.InnerHtml = sb.ToString();
            if (!string.IsNullOrEmpty(pagerScript))
                pagerScript = "<script language=\"javascript\" type=\"text/javascript\">" + pagerScript + "</script>";
            return CopyrightText + pagerScript + tb.ToString(TagRenderMode.Normal) + CopyrightText;
        }

        private string BuildGoToPageSection(ref string pagerScript)
        {
            const string ctrlIndexName = "_MvcPager_ControlIndex";
            ViewContext viewCtx = _msAjaxPaging
                                      ? _ajax.ViewContext
                                      : _html.ViewContext;
            int ctrlIndex;
            if (int.TryParse((string)viewCtx.HttpContext.Items[ctrlIndexName], out ctrlIndex))
                ++ctrlIndex;
            viewCtx.HttpContext.Items[ctrlIndexName] = ctrlIndex.ToString();

            string controlId = "_MvcPager_Ctrl" + ctrlIndex;
            string scriptLink = GenerateAnchor(new PagerItem("0", 0, false, PagerItemType.NumericPage));
            string linkClickTrigger = "_hl.click()";
            string browserName = viewCtx.HttpContext.Request.Browser.Browser.ToLower();
            if (browserName.Contains("safari") || browserName.Contains("firefox"))
                linkClickTrigger =
                    "var evt=document.createEvent('MouseEvents');evt.initEvent('click',true,true);_hl.dispatchEvent(evt)";

            if (ctrlIndex == 0)
            {
                pagerScript += KeyDownScript.Replace("%TotalPageCount%", _totalPageCount.ToString()) + GoToPageScript.Replace("%InvalidPageIndexErrorMessage%",
                                                                      _pagerOptions.InvalidPageIndexErrorMessage).Replace("%PageIndexOutOfRangeErrorMessage%",
                                                       _pagerOptions.PageIndexOutOfRangeErrorMessage).Replace("%ClickHandler%", linkClickTrigger);
            }
            string onChangeScript = null;
            if (!_pagerOptions.ShowGoButton)
                onChangeScript = " onchange=\"_MvcPager_GoToPage(this," + _totalPageCount + ")\"";
            StringBuilder piBuilder = new StringBuilder();
            if (_pagerOptions.PageIndexBoxType == PageIndexBoxType.DropDownList)
            {
                // start page index
                int startIndex = _pageIndex - (_pagerOptions.MaximumPageIndexItems / 2);
                if (startIndex + _pagerOptions.MaximumPageIndexItems > _totalPageCount)
                    startIndex = _totalPageCount + 1 - _pagerOptions.MaximumPageIndexItems;
                if (startIndex < 1)
                    startIndex = 1;

                // end page index
                int endIndex = startIndex + _pagerOptions.MaximumPageIndexItems - 1;
                if (endIndex > _totalPageCount)
                    endIndex = _totalPageCount;
                piBuilder.AppendFormat("<select id=\"{0}\"{1}>", controlId + "_pib", onChangeScript);
                for (int i = startIndex; i <= endIndex; i++)
                {
                    piBuilder.AppendFormat("<option value=\"{0}\"", i);
                    if (i == _pageIndex)
                        piBuilder.Append(" selected=\"selected\"");
                    piBuilder.AppendFormat(">{0}</option>", i);
                }
                piBuilder.Append("</select>");
            }
            else
                piBuilder.AppendFormat(
                    "<input type=\"text\" id=\"{0}\" value=\"{1}\" onkeydown=\"_MvcPager_Keydown(event)\"{2}/>",
                    controlId + "_pib", _pageIndex, onChangeScript);
            string outHtml;
            if (!string.IsNullOrEmpty(_pagerOptions.PageIndexBoxWrapperFormatString))
            {
                outHtml = string.Format(_pagerOptions.PageIndexBoxWrapperFormatString, piBuilder);
                piBuilder = new StringBuilder(outHtml);
            }

            if (_pagerOptions.ShowGoButton)
                piBuilder.AppendFormat(
                    "<input type=\"button\" value=\"{0}\" onclick=\"_MvcPager_GoToPage(document.getElementById('{1}')," + _totalPageCount + ")\"/>",
                    _pagerOptions.GoButtonText
                    , controlId + "_pib");
            piBuilder.AppendFormat("<span id=\"{0}\" style=\"display:none;width:0px;height:0px\">{1}</span>",
                                   controlId + "_piblink", scriptLink);
            if (!string.IsNullOrEmpty(_pagerOptions.GoToPageSectionWrapperFormatString) ||
                !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString))
            {
                outHtml = string.Format(
                    _pagerOptions.GoToPageSectionWrapperFormatString ?? _pagerOptions.PagerItemWrapperFormatString,
                    piBuilder);
            }
            else
                outHtml = piBuilder.ToString();
            return outHtml;
        }


        private string GenerateAnchor(PagerItem item)
        {
            if (_msAjaxPaging)
            {
                var routeValues = GetCurrentRouteValues(_ajax.ViewContext);
                if (item.PageIndex == 0)
                    routeValues[_pagerOptions.PageIndexParameterName] = ScriptPageIndexName;
                else
                    routeValues[_pagerOptions.PageIndexParameterName] = item.PageIndex;
                if (!string.IsNullOrEmpty(_routeName))
                    return _ajax.RouteLink(item.Text, _routeName, routeValues, _ajaxOptions).ToHtmlString();
                return _ajax.RouteLink(item.Text, routeValues, _ajaxOptions).ToHtmlString();
            }
            string url = GenerateUrl(item.PageIndex);
            if (_pagerOptions.UseJqueryAjax)
            {
                StringBuilder ehBuilder = new StringBuilder();
                //ignore OnSuccess property
                if (!string.IsNullOrEmpty(_ajaxOptions.OnFailure) || !string.IsNullOrEmpty(_ajaxOptions.OnBegin))
                {
                    ehBuilder.Append("$.ajax({url:$(this).attr(\'href\'),success:function(data,status,xhr){$(\'#");
                    ehBuilder.Append(_ajaxOptions.UpdateTargetId).Append("\').html(data);}");
                    if (!string.IsNullOrEmpty(_ajaxOptions.OnFailure))
                        ehBuilder.Append(",error:").Append(HttpUtility.HtmlAttributeEncode(_ajaxOptions.OnFailure));
                    if (!string.IsNullOrEmpty(_ajaxOptions.OnBegin))
                        ehBuilder.Append(",beforeSend:").Append(HttpUtility.HtmlAttributeEncode(_ajaxOptions.OnBegin));
                    if (!string.IsNullOrEmpty(_ajaxOptions.OnComplete))
                        ehBuilder.Append(",complete:").Append(
                            HttpUtility.HtmlAttributeEncode(_ajaxOptions.OnComplete));
                    ehBuilder.Append("});return false;");
                }
                else
                {
                    ehBuilder.Append("$(\'#").Append(_ajaxOptions.UpdateTargetId);
                    ehBuilder.Append("\').load($(this).attr(\'href\')");
                    if (!string.IsNullOrEmpty(_ajaxOptions.OnComplete))
                        ehBuilder.Append(",").Append(HttpUtility.HtmlAttributeEncode(_ajaxOptions.OnComplete));
                    ehBuilder.Append(");return false;");
                }
                return string.IsNullOrEmpty(url)
                           ? _html.Encode(item.Text)
                           : String.Format(CultureInfo.InvariantCulture,
                                           "<a href=\"{0}\" onclick=\"{1}\">{2}</a>",
                                           GenerateUrl(item.PageIndex), ehBuilder, item.Text);
            }
            return "<a href=\"" + url +
                   "\" onclick=\"window.open(this.attributes.getNamedItem('href').value,'_self')\"></a>";
        }

        private string GeneratePagerElement(PagerItem item)
        {
            //pager item link
            string url = GenerateUrl(item.PageIndex);
            if (item.Disabled) //first,last,next or previous page
                return CreateWrappedPagerElement(item, String.Format("<a disabled=\"disabled\">{0}</a>", item.Text));
            return CreateWrappedPagerElement(item,
                                             string.IsNullOrEmpty(url)
                                                 ? _html.Encode(item.Text)
                                                 : String.Format("<a href='{0}'>{1}</a>", url, item.Text));
        }

        private string GenerateJqAjaxPagerElement(PagerItem item)
        {
            if (item.Disabled)
                return CreateWrappedPagerElement(item, String.Format("<a disabled=\"disabled\">{0}</a>", item.Text));
            return CreateWrappedPagerElement(item, GenerateAnchor(item));
        }

        private string GenerateMsAjaxPagerElement(PagerItem item)
        {
            if (item.PageIndex == _pageIndex && !item.Disabled) //current page index
                return CreateWrappedPagerElement(item, item.Text);
            if (item.Disabled)
                return CreateWrappedPagerElement(item, string.Format("<a disabled=\"disabled\">{0}</a>", item.Text));

            // return null if current page index less than 1 or large than total page count
            if (item.PageIndex < 1 || item.PageIndex > _totalPageCount)
                return null;

            return CreateWrappedPagerElement(item, GenerateAnchor(item));
        }

        private string CreateWrappedPagerElement(PagerItem item, string el)
        {
            string navStr = el;
            switch (item.Type)
            {
                case PagerItemType.FirstPage:
                case PagerItemType.LastPage:
                case PagerItemType.NextPage:
                case PagerItemType.PrevPage:
                    if ((!string.IsNullOrEmpty(_pagerOptions.NavigationPagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString)))
                        navStr =
                            string.Format(
                                _pagerOptions.NavigationPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
                case PagerItemType.MorePage:
                    if ((!string.IsNullOrEmpty(_pagerOptions.MorePagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString)))
                        navStr =
                            string.Format(
                                _pagerOptions.MorePagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
                case PagerItemType.NumericPage:
                    if (item.PageIndex == _pageIndex &&
                        (!string.IsNullOrEmpty(_pagerOptions.CurrentPagerItemWrapperFormatString) ||
                         !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString))) //current page
                        navStr =
                            string.Format(
                                _pagerOptions.CurrentPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    else if (!string.IsNullOrEmpty(_pagerOptions.NumericPagerItemWrapperFormatString) ||
                             !string.IsNullOrEmpty(_pagerOptions.PagerItemWrapperFormatString))
                        navStr =
                            string.Format(
                                _pagerOptions.NumericPagerItemWrapperFormatString ??
                                _pagerOptions.PagerItemWrapperFormatString, el);
                    break;
            }
            return navStr + _pagerOptions.SeparatorHtml;
        }


        private RouteValueDictionary GetCurrentRouteValues(ViewContext viewContext)
        {
            RouteValueDictionary mergedRouteValues = RouteValuesHelpers.MergeRouteValues(_actionName, _controllerName, _routeValues, viewContext.RouteData.Values, true);
            var rq = viewContext.HttpContext.Request.QueryString;
            foreach (string key in rq.Keys)
            {
                // add other url query string parameters (not include PageIndexParameterName parameter value and X-Requested-With=XMLHttpRequest ajax parameter) to route value collection
                if (key != _pagerOptions.PageIndexParameterName &&
                    (key.ToLower() != "x-requested-with" && rq[key].ToLower() != "xmlhttprequest"))
                {
                    if (!mergedRouteValues.ContainsKey(key)) mergedRouteValues[key] = rq[key];
                }
            }

            //// action
            //routeValues["action"] = _actionName;
            //// controller
            //routeValues["controller"] = _controllerName;
            return mergedRouteValues;
        }
    }
    internal static class RouteValuesHelpers
    {
        public static RouteValueDictionary GetRouteValues(RouteValueDictionary routeValues)
        {
            return (routeValues != null) ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
        }

        public static RouteValueDictionary MergeRouteValues(string actionName, string controllerName, RouteValueDictionary implicitRouteValues, RouteValueDictionary routeValues, bool includeImplicitMvcValues)
        {
            // Create a new dictionary containing implicit and auto-generated values
            RouteValueDictionary mergedRouteValues = new RouteValueDictionary();

            if (includeImplicitMvcValues)
            {
                // We only include MVC-specific values like 'controller' and 'action' if we are generating an action link.
                // If we are generating a route link [as to MapRoute("Foo", "any/url", new { controller = ... })], including
                // the current controller name will cause the route match to fail if the current controller is not the same
                // as the destination controller.

                object implicitValue;
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("action", out implicitValue))
                {
                    mergedRouteValues["action"] = implicitValue;
                }

                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("controller", out implicitValue))
                {
                    mergedRouteValues["controller"] = implicitValue;
                }

                foreach (KeyValuePair<string, object> routeElement in GetRouteValues(implicitRouteValues))
                {
                    if (!routeElement.Key.Equals("action", StringComparison.OrdinalIgnoreCase) && !routeElement.Key.Equals("controller", StringComparison.OrdinalIgnoreCase))
                        mergedRouteValues[routeElement.Key] = routeElement.Value;
                }
            }

            // Merge values from the user's dictionary/object
            if (routeValues != null)
            {
                foreach (KeyValuePair<string, object> routeElement in GetRouteValues(routeValues))
                {
                    mergedRouteValues[routeElement.Key] = routeElement.Value;
                }
            }

            // Merge explicit parameters when not null
            if (actionName != null)
            {
                mergedRouteValues["action"] = actionName;
            }

            if (controllerName != null)
            {
                mergedRouteValues["controller"] = controllerName;
            }

            return mergedRouteValues;
        }

        public static void Merge(this RouteValueDictionary obj, RouteValueDictionary routeValues)
        {
            foreach (KeyValuePair<string, object> routeElement in GetRouteValues(routeValues))
            {
                if (obj.ContainsKey(routeElement.Key))
                {
                    obj[routeElement.Key] = routeElement.Value;
                }
                else
                {
                    obj.Add(routeElement.Key, routeElement.Value);
                }
            }
        }
    }
}