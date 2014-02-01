using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace RuntimeIt1.Controllers
{
    public class Parser
    {

        public Parser()
        {

        }
        public string Parse(string Operand, string Variables)
        {
            string[] VariablesArray = Variables.Split(new Char[] { '/' });
            int Iterator = 0;
            string Solution = Operand;

            foreach (string Variable in VariablesArray)
            {
                Iterator++;
                Regex regex = new Regex("var");
                Solution = regex.Replace(Solution, Variable, 1);   

            }
            return Solution;
        }
    }
}