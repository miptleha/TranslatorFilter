using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Translator
{
    public class TranslatorFilter : ActionFilterAttribute, IResultFilter
    {
        public TranslatorFilter()
        {
        }

        List<string> _ignoreAction = new List<string>();
        public TranslatorFilter(string[] ignoreAction)
        {
            foreach (var s in ignoreAction)
                _ignoreAction.Add(s.ToLower());
        }

        void IResultFilter.OnResultExecuted(ResultExecutedContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();
            if (!string.IsNullOrEmpty(action) && _ignoreAction.Contains(action.ToLower()))
                return;
            filterContext.HttpContext.Response.Filter = new TranslatorFilterStream(filterContext.HttpContext.Response.Filter);
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}