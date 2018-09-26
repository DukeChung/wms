using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Helpers
{
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass)) 
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        /// <summary>
        /// 页面导航
        /// </summary>
        /// <param name="html"></param>
        /// <param name="moduleName"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public static HtmlString Breadcrumb(this HtmlHelper html,string moduleName,string functionName)
        {
            var strHtml = new StringBuilder();
            strHtml.Append(" <div class=\"row wrapper border-bottom white-bg page-heading\">");
            strHtml.Append(" <div class=\"col-lg-10\"> <h2></h2>");
            strHtml.Append("<ol class=\"breadcrumb\">");
            strHtml.AppendFormat("<li><a href =\"{0}\" > 首页 </a> </li>", "/Home/Index");
            strHtml.AppendFormat(" <li><a>{0}</a></li><li class=\"active\"><strong>{1}</strong></li>", moduleName, functionName);
            strHtml.Append("</ol> </div> <div class=\"col-lg-2\"></div></div>");
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 页面导航
        /// </summary>
        /// <param name="html"></param>
        /// <param name="moduleName"></param>
        /// <param name="moduleAction">模块Action路径</param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public static HtmlString Breadcrumb(this HtmlHelper html, string moduleName,string moduleAction, string functionName)
        {
            var strHtml = new StringBuilder();
            strHtml.Append(" <div class=\"row wrapper border-bottom white-bg page-heading\">");
            strHtml.Append(" <div class=\"col-lg-10\"> <h2></h2>");
            strHtml.Append("<ol class=\"breadcrumb\">");
            strHtml.AppendFormat("<li><a href =\"{0}\" > 首页 </a> </li>", "Home/Index");
            strHtml.AppendFormat(" <li><a href =\"{0}\" >{1}</a></li><li class=\"active\"><strong>{2}</strong></li>", moduleAction,moduleName, functionName);
            strHtml.Append("</ol> </div> <div class=\"col-lg-2\"></div></div>");
            return new HtmlString(strHtml.ToString());
        }


        /// <summary>
        /// 查询控件
        /// </summary>
        /// <param name="html"></param>
        /// <param name="lableName">名称</param>
        /// <param name="textId">控件ID</param>
        /// <returns></returns>
        public static HtmlString SearchTextBox(this HtmlHelper html, string lableName, string textId)
        {
            var strHtml = new StringBuilder();
            strHtml.Append(" <div class=\"form-group\">");
            strHtml.AppendFormat(" <label class=\"control-label\" for=\"{1}\">{0}</label>", lableName, textId);
            strHtml.AppendFormat(" <input type=\"text\" id=\"{0}\" name=\"{0}\" value=\"\" placeholder=\"请输入{1}\" class=\"form-control\">  </div>", textId, lableName);
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 时间控件,只处理控件的HTML 其他JS 和样式需要单独引用
        /// Styles.Render("~/plugins/dataPickerStyles")
        /// Scripts.Render("~/plugins/dataPicker")
        /// 控件JS 初始化ID 自动增加 div 前缀 div+textId
        /// 取值正常使用textId
        ///     $('#divExpiryDate .input-group.date').datepicker({
        ///         todayBtn: "linked",
        ///  keyboardNavigation: false,
        ///  forceParse: false,
        ///  calendarWeeks: true,
        ///  autoclose: true
        ///  });
        /// </summary>
        /// <param name="html"></param>
        /// <param name="textId"></param>
        /// <returns></returns>
        public static HtmlString DataPickerTextBox(this HtmlHelper html, string lableName,string textId)
        {
            var strHtml = new StringBuilder();
            strHtml.AppendFormat(" <label class=\"control-label\" for=\"{1}\">{0}</label>", lableName, textId);
            strHtml.AppendFormat(" <div class=\"form-group\" id=\"div{0}\"  >", textId);
            strHtml.Append("<div class=\"input-group date\"><span class=\"input-group-addon\"> <i class=\"fa fa-calendar\"></i></span> ");
            strHtml.AppendFormat(" <input type =\"text\" class=\"form-control\" id=\"{0}\" name=\"{0}\" value='' placeholder=\"请输入{1}\"  ></div></div>", textId,lableName);
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 打印页 使用 文字下方的线____ 
        /// </summary>
        /// <returns></returns>
        public static HtmlString PrintLine(this HtmlHelper html)
        {
            var strHtml = new StringBuilder();
            strHtml.Append(" <div class=\"form-group hr-line-dashed\"> <div class=\"col-sm-12\"> </div> </div>  ");
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 打印模板表单头部布局
        /// </summary>
        /// <param name="html"></param>
        /// <param name="printTitle"></param>
        /// <param name="printValue"></param>
        /// <returns></returns>
        public static HtmlString PrintTitle(this HtmlHelper html,string printTitle,string printValue,int length=4)
        {

            var strHtml = new StringBuilder();
            strHtml.AppendFormat("<div class='col-xs-{0}'>", length);
            strHtml.AppendFormat("<div class='row'> <div class='col-xs-5 PrintTitle'> {0}:</div> <div class='col-xs-7 PrintTitleValue'>{1}</div>  </div>", printTitle, printValue);
            strHtml.Append(" <div class='row'>   <div class='col-xs-5 ' style='margin-top: 5px'> </div>  <div class='col-xs-6 line-dashed' style='margin-top: 5px;margin-left: -10px'> </div><div class='col-xs-1 ' style='margin-top: 5px'> </div>  </div>  </div>");
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 打印模板表单头部布局
        /// </summary>
        /// <param name="html"></param>
        /// <param name="printTitle"></param>
        /// <param name="printValue"></param>
        /// <returns></returns>
        public static HtmlString PrintLongTitle(this HtmlHelper html, string printTitle, string printValue, int length = 4)
        {

            var strHtml = new StringBuilder();
            strHtml.AppendFormat("<div class='col-xs-{0}'>", length);
            strHtml.AppendFormat("<div class='row'> <div class='col-xs-7 PrintTitle'> {0}:</div> <div class='col-xs-5 PrintTitleValue'>{1}</div>  </div>", printTitle, printValue);
            strHtml.Append(" <div class='row'>   <div class='col-xs-5 ' style='margin-top: 5px'> </div>  <div class='col-xs-4 line-dashed' style='margin-top: 5px;margin-left: 30px'> </div><div class='col-xs-1 ' style='margin-top: 5px'> </div>  </div>  </div>");
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 打印TD样式控制,打印控件无法加载table外部样式
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tdText"></param>
        /// <returns></returns>
        public static HtmlString PrintTd(this HtmlHelper html,string tdText=null)
        {
            var strHtml = new StringBuilder();
            strHtml.AppendFormat(
                "  <td  style='padding: 8px;line-height: 1.42857143;vertical-align: top;border-top: 1px solid #ddd;text-align: left;font-family: 宋体;font-size: 10px;'>{0}</td>",
                tdText);
            return new HtmlString(strHtml.ToString());
        }

        /// <summary>
        /// 打印Th样式控制,打印控件无法加载table外部样式
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tdText"></param>
        /// <returns></returns>
        public static HtmlString PrintTh(this HtmlHelper html, string tdText)
        {
            var strHtml = new StringBuilder();
            strHtml.AppendFormat(
                "  <td  style='padding: 8px;line-height: 1.42857143;vertical-align: top;border-top: 1px solid #ddd;text-align: left;font-family: 宋体;font-size: 10px;'>{0}</td>",
                tdText);
            return new HtmlString(strHtml.ToString());
        }
    }
}
