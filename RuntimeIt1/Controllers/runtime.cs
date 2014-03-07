using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic;
using RuntimeIt1.Exceptions;
using System.Net;
namespace RuntimeIt1.Controllers
{
    public class Runtime
    {
        private ScriptEngine runtime;
        public Runtime()
        {
            runtime = new Jurassic.ScriptEngine();
        }

        public object Execute(string query)
        {
            try
            {
                return runtime.Evaluate(query);
            }
            catch (Jurassic.JavaScriptException)
            {
                throw new ServiceException(HttpStatusCode.BadRequest,"Runtime error check parameter values");
            }
        }

    }

}