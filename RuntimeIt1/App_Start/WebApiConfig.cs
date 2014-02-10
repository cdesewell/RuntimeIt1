﻿using System;
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
                routeTemplate: "runtime",
                defaults: new { controller = "runtime", action = "GetFunctions" }
                );

            config.Routes.MapHttpRoute(
                name: "ProblemParameters",
                routeTemplate: "runtime/{problem}",
                defaults: new { controller = "runtime", action = "GetParameters" }
                );

            config.Routes.MapHttpRoute(
                name: "LoopedProblemDelegation",
                routeTemplate: "runtime/loop/{accumulator}/{start}/{finish}/{iterate}/{problem}/{*variables}",
                defaults: new { controller = "runtime", action = "GerLoopedSolution" }
                );

            config.Routes.MapHttpRoute(
                name: "ProblemDefinition",
                routeTemplate: "runtime/define/{name}",
                defaults: new { controller = "runtime", action = "SetProblem" }
                );

            config.Routes.MapHttpRoute(
                name: "ProblemDelegation",
                routeTemplate: "runtime/{problem}/{*variables}",
                defaults: new { controller = "runtime", action = "GetSolution" }
                );



            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Loop",
                routeTemplate: "{controller}/loop/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "Define",
            routeTemplate: "{controller}/define/{id}",
            defaults: new { id = RouteParameter.Optional }
);
        }
    }
}
