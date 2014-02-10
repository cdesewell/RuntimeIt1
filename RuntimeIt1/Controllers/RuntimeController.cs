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

        ProblemDictinary ProblemDictionary = HttpContext.Current.Application["ProblemDictionary"] as ProblemDictinary;
        Parser Parser = new Parser();
        Runtime JavaScriptEngine = new Runtime();
        XmlDocument ResponseBody;

        [HttpGet]//GET api/runtime
        public HttpResponseMessage GetFunctions()
        {
            List<string> Functions = this.ProblemDictionary.GetFunctions();

            ResponseBody = new XmlDocument();
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

            ResponseBody = new XmlDocument();
            XmlElement rootElement = ResponseBody.CreateElement("Parameters");
            rootElement.InnerText = Convert.ToString(this.Parser.GetParameterNo(Solution));
            ResponseBody.AppendChild(rootElement);

            return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Parameters.xsd");
        }

        [HttpGet]//GET runtime/{Function}/{Parameter} ...
        public HttpResponseMessage GetSolution(string Problem, string Variables)
        {
            string Solution = this.Parser.Parse(this.ProblemDictionary.GetProblem(Problem), Variables);

            ResponseBody = new XmlDocument();
            XmlElement RootElement = ResponseBody.CreateElement("Return");
            RootElement.InnerText = this.JavaScriptEngine.Execute(Solution).ToString();
            ResponseBody.AppendChild(RootElement);

            return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
        }

        [HttpGet]//GET runtime/loop/{start}/{finish}/{iterate}/{Function}/{Parameter} ...
        public HttpResponseMessage GerLoopedSolution(int Start, int Finish, int Iterate, string Accumulator, string Problem, string Variables)
        {
            string BaseSolution = this.JavaScriptEngine.Execute(Parser.Parse(ProblemDictionary.GetProblem(Problem), Variables)).ToString();
            string Solution = BaseSolution;

            for (int x = Start; x < Finish; x += Iterate)
            {
                Solution += "/" + this.JavaScriptEngine.Execute(this.Parser.Parse(ProblemDictionary.GetProblem(Problem), Variables)).ToString();
                Solution = this.Parser.Parse(this.ProblemDictionary.GetProblem(Accumulator), Solution);
            }

            Solution = this.Parser.Stringify(BaseSolution, Solution);

            ResponseBody = new XmlDocument();
            XmlElement RootElement = ResponseBody.CreateElement("Return");
            RootElement.InnerText = this.JavaScriptEngine.Execute(Solution).ToString();
            ResponseBody.AppendChild(RootElement);

            return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
        }

        [HttpPost]
        public void SetProblem(string Name)
        {
            StreamReader RequestBodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            XmlDocument XmlDefinition = new XmlDocument(); // Create an XML document object
            XmlDefinition.LoadXml(RequestBodyStream.ReadToEnd());
            XmlNodeList Root = XmlDefinition.GetElementsByTagName("Problem-definition");
            XmlNodeList Function = Root[0].ChildNodes;
            string Solution = RecurseDefineProblem(Function, "");
            this.ProblemDictionary.SetProblem(Name, Solution);

        }

        // PUT api/runtime/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/runtime/5
        public void Delete(int id)
        {
        }


        private string RecurseDefineProblem(XmlNodeList Function, string Solution)
        {
            XmlAttributeCollection FunctionName = Function[0].Attributes;
            string Name = FunctionName["name"].Value;
            string SubSolution = this.ProblemDictionary.GetProblem(Name);
            XmlNodeList Variables = Function[0].ChildNodes;
            int ParameterNo = Parser.GetParameterNo(SubSolution);
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
                         return Parser.ParseNo(Solution, 
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
            return new HttpResponseMessage() { Content = new StringContent(ResponseBody.OuterXml, new UTF8Encoding(), "application/xml") };
        }


        private void BuildError()
        {

        }
    }
}
