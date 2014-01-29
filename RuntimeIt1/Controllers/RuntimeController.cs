using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RuntimeIt1.Controllers
{
    public class RuntimeController : ApiController
    {
        ProblemDictinary ProblemDictionary;
        Parser SulutionParser;
        Runtime JavaScriptEngine;

        [HttpGet]
        public object SolveProblem(string Problem, string Variables)
        {
            ProblemDictionary = new ProblemDictinary();
            SulutionParser = new Parser();
            JavaScriptEngine = new Runtime();
            string Solution = SulutionParser.Parse(ProblemDictionary.GetProblem(Problem),Variables);
            return JavaScriptEngine.Execute(Solution);
        }

        // GET api/runtime/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/runtime
        public void Post([FromBody]string value)
        {
        }

        // PUT api/runtime/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/runtime/5
        public void Delete(int id)
        {
        }
    }
}
