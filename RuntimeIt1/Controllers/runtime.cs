using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic;
namespace RuntimeIt1.Controllers
{
    public class Runtime
    {
        ScriptEngine runtime;

        public Runtime()
        {
            runtime = new Jurassic.ScriptEngine();
        }

        public object Execute(string query)
        {
            return runtime.Evaluate(query);
        }

    }

 
}