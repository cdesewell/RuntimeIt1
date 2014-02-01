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
                name: "RuntimeFunctions",
                routeTemplate: "api/runtime",
                defaults: new { controller = "runtime", action = "GetFunctions" }
                );

            config.Routes.MapHttpRoute(
                name: "ProblemParameters",
                routeTemplate: "api/runtime/{problem}",
                defaults: new { controller = "runtime", action = "GetParameters" }
                );

            config.Routes.MapHttpRoute(
                name: "ProblemDelegation",
                routeTemplate: "api/runtime/{problem}/{*variables}",
                defaults: new { controller = "runtime", action = "GetSolution" }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
