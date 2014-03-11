using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic;
using System.Net;

using RuntimeIt1.Exceptions;

namespace RuntimeIt1.Controllers
{
    public class Runtime
    {
        private ScriptEngine runtime;
        public Runtime()
        {
            runtime = new Jurassic.ScriptEngine();
        }

        public string Execute(string query)
        {
            try
            {
                return runtime.Evaluate(query).ToString();
            }
            catch (Jurassic.JavaScriptException)
            {
                throw new ServiceException(HttpStatusCode.BadRequest,"Runtime error check parameter values");
            }
        }

    }

}