using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuizSystem.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "UserQuizzesDefault",
               url: "UserQuizzes/{quizId}/Questions/{action}",
               defaults: new { controller = "UserQuizzes" },
               namespaces: new[] { "QuizSystem.Web.Controllers" }

           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "QuizSystem.Web.Controllers" }
            );
        }
    }
}
