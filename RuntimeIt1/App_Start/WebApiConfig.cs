using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RuntimeIt1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "Problem",
                routeTemplate: "api/runtime/{problem}/{*variables}",
                defaults: new { controller = "runtime", action = "SolveProblem" }
                );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
