using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Text;
using RuntimeIt1.Logging;

namespace RuntimeIt1.Controllers
{
    public class RuntimeController : ApiController
    {
        
        ProblemDictinary ProblemDictionary = new ProblemDictinary();
        Parser SulutionParser;
        Runtime JavaScriptEngine;

        [HttpGet]//GET api/runtime
        public HttpResponseMessage GetFunctions()
        {
            XmlDocument ResponseBody = new XmlDocument();
            List<string>  Functions = this.ProblemDictionary.GetFunctions();
            ResponseBody = new XmlDocument();
            XmlElement FunctionRoot = ResponseBody.CreateElement("Functions");

            foreach (string FunctionName in Functions)
            {
                XmlElement FunctionElement = ResponseBody.CreateElement("Function");
                FunctionElement.SetAttribute("Name", FunctionName);
                FunctionRoot.AppendChild(FunctionElement);
            }

            ResponseBody.AppendChild(FunctionRoot);
            //ServiceLogger.LOG("First Entry");
            return BuildResponse(ResponseBody);
        }

        [HttpGet]//GET api/runtime/{Function}
        public HttpResponseMessage GetParameters(string Problem)
        {
            XmlDocument ResponseBody = new XmlDocument();
            int ParameterNo = this.ProblemDictionary.GetParameters(Problem);

            ResponseBody = new XmlDocument();
            XmlElement Parameters = ResponseBody.CreateElement("Parameters");
            Parameters.InnerText = ParameterNo.ToString();
            ResponseBody.AppendChild(Parameters);

            return BuildResponse(ResponseBody);
        }

        [HttpGet]//GET api/runtime/{Function}/{Parameter} ...
        public HttpResponseMessage GetSolution(string Problem, string Variables)
        {
            this.SulutionParser = new Parser();
            this.JavaScriptEngine = new Runtime();
            XmlDocument ResponseBody = new XmlDocument();
            string Solution = SulutionParser.Parse(ProblemDictionary.GetProblem(Problem), Variables);

            XmlElement Return = ResponseBody.CreateElement("Return");
            Return.InnerText = JavaScriptEngine.Execute(Solution).ToString();
            ResponseBody.AppendChild(Return);

            return this.BuildResponse(ResponseBody);
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
        private HttpResponseMessage BuildResponse(XmlDocument ResponseBody)
        {
            return new HttpResponseMessage() { Content = new StringContent(ResponseBody.OuterXml, new UTF8Encoding(), "application/xml") };
        }
        private void BuildError()
        {

        }
    }
}
