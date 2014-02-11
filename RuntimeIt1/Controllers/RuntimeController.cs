using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Xml;
using System.Text;
using RuntimeIt1.Logging;
using System.Xml.Schema;
using System.IO;
using System.Web.Http.Controllers;

namespace RuntimeIt1.Controllers
{
    public class RuntimeController : ApiController
    {

        private ProblemDictinary ProblemDictionary =
        HttpContext.Current.Application["ProblemDictionary"] as ProblemDictinary;
        private Parser Parser =
        HttpContext.Current.Application["Parser"] as Parser;
        private Runtime JavaScriptEngine =
        HttpContext.Current.Application["Runtime"] as Runtime;
        private XmlDocument ResponseBody = new XmlDocument();

        [HttpGet]//GET api/runtime
        public HttpResponseMessage GetFunctions()
        {
            List<string> Functions = this.ProblemDictionary.GetFunctions();

            XmlElement RootElement = ResponseBody.CreateElement("Functions");

            foreach (string FunctionKey in Functions)
            {
                XmlElement FunctionElement = ResponseBody.CreateElement("Function");
                FunctionElement.SetAttribute("Name", FunctionKey);
                RootElement.AppendChild(FunctionElement);
            }

            ResponseBody.AppendChild(RootElement);
            return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Functions.xsd");
        }

        [HttpGet]//GET api/runtime/{Function}
        public HttpResponseMessage GetParameters(string Problem)
        {
            string Solution = this.ProblemDictionary.GetProblem(Problem);

            XmlElement RootElement = ResponseBody.CreateElement("Parameters");
            RootElement.InnerText = Convert.ToString(this.Parser.GetParameterNo(Solution));
            ResponseBody.AppendChild(RootElement);

            return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Parameters.xsd");
        }

        [HttpGet]//GET runtime/{Function}/{Parameter} ...
        public HttpResponseMessage GetSolution(string Problem, string Variables)
        {
            string Solution = this.Parser.Parse(this.ProblemDictionary.GetProblem(Problem), Variables);

            XmlElement RootElement = ResponseBody.CreateElement("Return");
            RootElement.InnerText = this.JavaScriptEngine.Execute(Solution).ToString();
            ResponseBody.AppendChild(RootElement);

            return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
        }

        [HttpGet]//GET runtime/loop/{start}/{finish}/{iterate}/{Function}/{Parameter} ...
        public HttpResponseMessage GerLoopedSolution(int Start, int Finish, int Iterate, string Accumulator, string Problem, string Variables)
        {
            string Solution = this.JavaScriptEngine.Execute(
                this.Parser.Parse(
                this.ProblemDictionary.GetProblem(
                Problem),
                Variables)).ToString();

            string SubSolution = Solution;

            for (int x = Start; x < Finish; x += Iterate)
            {
                SubSolution += "/" + this.JavaScriptEngine.Execute(
                    this.Parser.Parse(
                    this.ProblemDictionary.GetProblem(
                    Problem),
                    Variables)).ToString();

                SubSolution = this.Parser.Parse(
                    this.ProblemDictionary.GetProblem(
                    Accumulator),
                    SubSolution);
            }

            SubSolution = this.Parser.Stringify(Solution, SubSolution);

            XmlElement RootElement = ResponseBody.CreateElement("Return");
            RootElement.InnerText = this.JavaScriptEngine.Execute(SubSolution).ToString();
            ResponseBody.AppendChild(RootElement);

            return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
        }

        //POST /runtime/{FUnction}
        [HttpPost]
        public HttpResponseMessage SetProblem(string Name)
        {
            XmlDocument XmlDefinition = new XmlDocument();
            XmlDefinition.LoadXml(
                new StreamReader(
                    HttpContext.Current.Request.InputStream).ReadToEnd(
                    ));

            XmlNodeList Function = XmlDefinition.GetElementsByTagName(
                "Problem-definition")[0].ChildNodes;

            string Solution = RecurseDefineProblem(Function, "");
            this.ProblemDictionary.SetProblem(Name, Solution);

            return this.BuildResponse(XmlDefinition, "http://myschema.com");
        }

        // PUT api/runtime/{Function}
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE /runtime/{Function}
        public HttpResponseMessage Delete(string Problem)
        {
            XmlElement RootElement = ResponseBody.CreateElement("Removed");

            this.ProblemDictionary.DeleteFunction(Problem);

            RootElement.InnerText = Problem;
            ResponseBody.AppendChild(RootElement);
            return BuildResponse(ResponseBody, "http://myschema.com");
        }


        private string RecurseDefineProblem(XmlNodeList Function, string Solution)
        {
            string Name = Function[0].Attributes["name"].Value;
            string SubSolution = this.ProblemDictionary.GetProblem(Name);
            XmlNodeList Variables = Function[0].ChildNodes;
            int ParameterNo = this.Parser.GetParameterNo(SubSolution);

            if (Variables.Count == ParameterNo)
            {
                for (int Incerement = 0; Incerement < Variables.Count; Incerement++)
                {
                    if (Variables[Incerement].HasChildNodes == true)
                    {
                        if (Solution == "")
                        {
                            Solution = SubSolution;
                        }
                        SubSolution = this.Parser.ParseNo(SubSolution,
                             RecurseDefineProblem(Variables[Incerement].ChildNodes,
                             Solution),
                             Incerement);
                    }
                }
            }
            return SubSolution;
        }

        private HttpResponseMessage BuildResponse(XmlDocument ResponseBody, string SchemaName)
        {
            ResponseBody.DocumentElement.SetAttribute("xmlns", SchemaName);
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    ResponseBody.OuterXml,
                    new UTF8Encoding(), "application/xml")
            };
        }

        private void BuildError()
        {

        }
    }
}
