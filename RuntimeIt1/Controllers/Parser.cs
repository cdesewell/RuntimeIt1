using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RuntimeIt1.Controllers
{
    public class Parser
    {

        public Parser()
        {

        }
        public string Parse(List<string> Operands, string Variables)
        {
            string[] VariablesArray = Variables.Split(new Char[] { '/' }, Operands.Count + 1);
            int Iterator = 0;
            string Solution = VariablesArray[Iterator];
            foreach (string Operand in Operands)
            {
                Iterator++;
                Solution += Operand + VariablesArray[Iterator];
            }
            return Solution;
        }
    }
}