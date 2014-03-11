using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Net;

using RuntimeIt1.Exceptions;

namespace RuntimeIt1.Controllers
{
    public class Parser
    {
        private Regex RegexVar;
        private Regex RegexString;
        public Parser(string VariableId, string StringId)
        {
            RegexVar = new Regex(VariableId);
            RegexString = new Regex(StringId);
        }
        public string Parse(string Problem, string Variables)
        {
            string[] VariablesArray = Variables.Split(new Char[] { '/' });

            if(VariablesArray.Length != this.GetParameterNo(Problem))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "Incorrect number of parameters");
            }

            foreach (string Variable in VariablesArray)
            {
                Problem = RegexVar.Replace(Problem, Variable, 1);
            }
            return Problem;
        }

        public string ParseNo(string Problem, string Variable, int No)
        {
            MatchCollection Matches = RegexVar.Matches(Problem);
            int VarialeIndex = Matches[No].Index;
            string Solution = Problem.Substring(0, VarialeIndex) + Variable;
            Solution += Problem.Substring(VarialeIndex + 3, Problem.Length - (VarialeIndex + 3));
            return Solution;
        }

        public int GetParameterNo(string Solution)
        {
            MatchCollection Vars;

            Vars = RegexVar.Matches(Solution);
            return Vars.Count;
        }

        public string Stringify(string BaseSolution, string Solution)
        {
            if (RegexString.IsMatch(Solution))
            {
                while (RegexString.IsMatch(Solution))
                {
                    Solution = RegexString.Replace(Solution, "", 1);
                }
                Solution = Solution.Replace(BaseSolution, "'" + BaseSolution + "'");
            }
            return Solution;
        }
        public string GetIncrement(string Variables, int Iterator)
        {
            string Solution = Variables.Replace("i", Iterator.ToString());
            return Solution;
        }
        public void ValidateDefinition(XmlDocument XmlDefinition)
        {
            XmlDefinition.Schemas.Add(null, "http://runtime.azurewebsites.net/Schemas/ProblemDefinition.xsd");
            XmlDefinition.Validate(
                (object sender, ValidationEventArgs e) =>
                {
                    throw new ServiceException(HttpStatusCode.BadRequest,"Problem definition could not be validated");
                }
                );
        }
    }
}