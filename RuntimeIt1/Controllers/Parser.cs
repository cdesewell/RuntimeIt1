using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;

namespace RuntimeIt1.Controllers
{
    public class Parser
    {

        public Parser()
        {

        }
        public string Parse(string Problem, string Variables)
        {
            string[] VariablesArray = Variables.Split(new Char[] { '/' });

            foreach (string Variable in VariablesArray)
            {
                Regex Regex = new Regex("var");
                Problem = Regex.Replace(Problem, Variable, 1);
            }
            return Problem;
        }

        public string ParseNo(string Problem, string Variable, int No)
        {
            Regex Regex = new Regex("var");
            MatchCollection Matches = Regex.Matches(Problem);
            int VarialeIndex = Matches[No].Index;
            string Solution = Problem.Substring(0, VarialeIndex) + Variable;
            if (VarialeIndex + 3 != Problem.Length)
            {
                Solution += Problem.Substring(VarialeIndex + 3, Problem.Length);
            }
            return Solution;
        }

        public int GetParameterNo(string Solution)
        {
            MatchCollection Vars;
            Regex Regex = new Regex("var");
            Vars = Regex.Matches(Solution);
            return Vars.Count;
        }

        public string Stringify(string BaseSolution, string Solution)
        {
            Regex Regex = new Regex("'");
            if (Regex.IsMatch(Solution))
            {
                while (Regex.IsMatch(Solution))
                {
                    Solution = Regex.Replace(Solution, "", 1);
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





    }
}