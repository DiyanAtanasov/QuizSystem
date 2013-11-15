using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Extensions.FiltersExtensions
{
    public class NoCache : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("Pragma", "no-cache");
            filterContext.HttpContext.Response.AddHeader("Cache-Control", "no-cache");
            filterContext.HttpContext.Response.AddHeader("Cache-Control", "must-revalidate");
            filterContext.HttpContext.Response.AddHeader("Expires", "Mon, 7 Jan 2013 00:00:00 GM");

            base.OnResultExecuted(filterContext);
        }
    }
}