using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Threading.Tasks;
using RuntimeIt1.Logging;
using RuntimeIt1.Controllers;

namespace RuntimeIt1
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            HttpContext.Current.Application["ProblemDictionary"] = new ProblemDictinary();
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var Context = sender as HttpApplication;
            new Task(() => ServiceLogger.LOG(Context.Request.UserHostAddress, Context.Request.HttpMethod, 
                Context.Request.Url.ToString())).Start();
        }
    }
}