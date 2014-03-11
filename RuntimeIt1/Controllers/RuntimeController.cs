using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Xml;
using System.Text;
using System.Xml.Schema;
using System.IO;
using System.Web.Http.Controllers;

using RuntimeIt1.Exceptions;
using RuntimeIt1.Models;

namespace RuntimeIt1.Controllers
{
    public class RuntimeController : ApiController
    {

        private ProblemDictinary ProblemDictionary =
        HttpContext.Current.Application["ProblemDictionary"] as ProblemDictinary;
        private Parser Parser =
        HttpContext.Current.Application["Parser"] as Parser;
        private Runtime Runtime =
        HttpContext.Current.Application["Runtime"] as Runtime;
        private XmlDocument ResponseBody = new XmlDocument();
        XmlElement RootElement;

        [HttpGet]//GET /api/runtime
        public HttpResponseMessage GetFunctions()
        {
            try
            {
                List<string> Functions = this.ProblemDictionary.GetFunctions();

                RootElement = ResponseBody.CreateElement("Functions");

                foreach (string FunctionKey in Functions)
                {
                    XmlElement FunctionElement = ResponseBody.CreateElement("Function");
                    FunctionElement.SetAttribute("Name", FunctionKey);
                    RootElement.AppendChild(FunctionElement);
                }

                ResponseBody.AppendChild(RootElement);
                return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Functions.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        [HttpGet]//GET /api/runtime/{Function}
        public HttpResponseMessage GetParameters(string Problem)
        {
            try
            {
                string Solution = this.ProblemDictionary.GetProblem(Problem);

                RootElement = ResponseBody.CreateElement("Parameters");
                RootElement.InnerText = Convert.ToString(this.Parser.GetParameterNo(Solution));
                ResponseBody.AppendChild(RootElement);

                return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Parameters.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        [HttpGet]//GET /runtime/{Function}/{Parameter} ...
        public HttpResponseMessage GetSolution(string Problem, string Variables)
        {
            try
            {
                string Solution = this.Parser.Parse(this.ProblemDictionary.GetProblem(Problem), Variables);

                RootElement = ResponseBody.CreateElement("Return");
                RootElement.InnerText = this.Runtime.Execute(Solution);
                ResponseBody.AppendChild(RootElement);

                return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        [HttpGet]//GET /runtime/loop/{start}/{finish}/{iterate}/{Function}/{Parameter} ...
        public HttpResponseMessage GerLoopedSolution(int Start, int Finish, int Iterate, string Accumulator, string Problem, string Variables)
        {
            try
            {
                string Solution = this.Runtime.Execute(
                    this.Parser.Parse(
                    this.ProblemDictionary.GetProblem(
                    Problem),
                    this.Parser.GetIncrement(Variables, Start)
                    ));

                string SubSolution = Solution;

                for (int x = Start; x < Finish; x += Iterate)
                {
                    SubSolution += "/" + this.Runtime.Execute(
                        this.Parser.Parse(
                        this.ProblemDictionary.GetProblem(
                        Problem),
                        this.Parser.GetIncrement(Variables, x)
                        ));

                    SubSolution = this.Parser.Parse(
                        this.ProblemDictionary.GetProblem(
                        Accumulator),
                        SubSolution);
                }

                SubSolution = this.Parser.Stringify(Solution, SubSolution);

                RootElement = ResponseBody.CreateElement("Return");
                RootElement.InnerText = this.Runtime.Execute(SubSolution);
                ResponseBody.AppendChild(RootElement);

                return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Return.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        [HttpPost]//POST /runtime/{Function}
        public HttpResponseMessage SetProblem(string Name)
        {
            try
            {
                XmlNodeList Function = GetRequestBody();
                string SubSolution = this.ProblemDictionary.GetProblem(Function[0].Attributes["name"].Value);

                string Solution = RecurseDefineProblem(Function, SubSolution);
                this.ProblemDictionary.SetProblem(Name, Solution);

                RootElement = ResponseBody.CreateElement("Problem");
                RootElement.InnerText = Name;
                ResponseBody.AppendChild(RootElement);

                return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Problem.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        [HttpPut]//PUT /runtime/refactor/{Function}
        [ActionName("Problem")]
        public HttpResponseMessage UpdateProblem(string Name)
        {
            try
            {
                XmlNodeList Function = GetRequestBody();
                string SubSolution = this.ProblemDictionary.GetProblem(Function[0].Attributes["name"].Value);

                string Solution = RecurseDefineProblem(Function, SubSolution);
                this.ProblemDictionary.UpdateProblem(Name, Solution);

                RootElement = ResponseBody.CreateElement("Problem");
                RootElement.InnerText = Name;
                ResponseBody.AppendChild(RootElement);

                return this.BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Problem.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }

        }

        [HttpDelete]//DELETE /runtime/{Function}
        [ActionName("Problem")]
        public HttpResponseMessage DeleteProblem(string Problem)
        {
            try
            {
                this.ProblemDictionary.DeleteProblem(Problem);

                RootElement = ResponseBody.CreateElement("Removed");
                RootElement.InnerText = Problem;
                ResponseBody.AppendChild(RootElement);
                return BuildResponse(ResponseBody, "http://runtime.azurewebsites.net/Schemas/Remove.xsd");
            }
            catch (ServiceException ServiceException)
            {
                return this.BuildError(ServiceException.HTTPCode, ServiceException.HTTPMessage);
            }
        }

        private XmlNodeList GetRequestBody()
        {
            XmlDocument XmlDefinition = new XmlDocument();
            try
            {
                XmlDefinition.LoadXml(
                    new StreamReader(
                        HttpContext.Current.Request.InputStream).ReadToEnd(
                        ));
            }
            catch (XmlException)
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "XML body is not well formed");
            }
            this.Parser.ValidateDefinition(XmlDefinition);

            return XmlDefinition.GetElementsByTagName(
                "Problem-definition")[0].ChildNodes;
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

        private HttpResponseMessage BuildError(HttpStatusCode ErrorCode, string ErrorMessage)
        {
            XmlDocument ErrorBody = new XmlDocument();
            RootElement = ErrorBody.CreateElement("Error");
            RootElement.InnerText = ErrorMessage;
            ErrorBody.AppendChild(RootElement);

            ErrorBody.DocumentElement.SetAttribute("xmlns", "http://runtime.azurewebsites.net/Schemas/Error.xsd");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    ErrorBody.OuterXml,
                    new UTF8Encoding(), "application/xml"),
                StatusCode = ErrorCode
            };
        }
    }
}
