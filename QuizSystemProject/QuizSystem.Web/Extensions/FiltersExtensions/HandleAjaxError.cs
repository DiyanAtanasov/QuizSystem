using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Extensions.FiltersExtensions
{
    public class HandleAjaxError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                Exception ex = filterContext.Exception;

                if (ex is HttpException)
                {
                    filterContext.HttpContext.Response.StatusCode = (ex as HttpException).GetHttpCode();
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 400;
                }

                filterContext.HttpContext.Response.Write(ex.Message);
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}