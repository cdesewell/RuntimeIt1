using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using RuntimeIt1.Configuration;
using System.Text.RegularExpressions;

namespace RuntimeIt1.Controllers
{

    public class ProblemDictinary
    {
        public Dictionary<string, object> ProblemDirectory;
        public List<object> Problem;
        public string Solution;
        public OperandConfigurationSection CoreOperands;
        public string Pattern = @"(var)";
        public ProblemDictinary()
        {
            ProblemDirectory = new Dictionary<string, object>();
            CoreOperands = ConfigurationManager.GetSection("RuntimeCoreOperands") as OperandConfigurationSection;

            foreach (OperandConfigurationElement Operand in CoreOperands.Operands)
            {
                ProblemDirectory.Add(Operand.Name, Operand.Symbol);
            }

        }

        private string GetProblemRecurse(List<object> ProblemList, string Solution)
        {

            foreach (object ProblemElement in ProblemList)
            {
                if (ProblemElement is string)
                {
                    Solution += ProblemElement;
                }
                else
                {
                    Solution = GetProblemRecurse(
                        ProblemElement as List<object>, Solution
                        );
                }

            }
            return Solution;
        }

        public string GetProblem(string ProblemKey)
        {

            if (ProblemDirectory[ProblemKey] is string)
            {
                Solution += ProblemDirectory[ProblemKey];

            }
            else
            {
                Solution = GetProblemRecurse(
                    ProblemDirectory[ProblemKey] as List<object>, Solution
                    );
            }
            return Solution;
        }

        public List<string> GetFunctions()
        {
            List<string> Functions = new List<string>();

            foreach (KeyValuePair<string, object> Pair in ProblemDirectory)
            {
                Functions.Add(Pair.Key);
            }
            return Functions;
        }

        public int GetParameters(string Function)
        {
            int ParameterCount = 0;
            foreach (KeyValuePair<string, object> Pair in ProblemDirectory)
            {
                if (Pair.Key == Function)
                {
                    if (Pair.Value is string)
                    {
                        foreach (Match RegMatch in Regex.Matches(Pair.Value as string, Pattern))
                        {
                            ParameterCount++;
                        }
                    }
                    else
                    {
                        ParameterCount = GetParametersRecurse(Pair.Value as List<object>);
                    }
                }
            }
            return ParameterCount;
        }

        private int GetParametersRecurse(List<object> FunctionList)
        {
            int ParameterCount = 0;
            foreach (object FunctionElement in FunctionList)
            {
                if (FunctionElement is string)
                {
                    foreach (Match RegMatch in Regex.Matches(FunctionElement as string, Pattern))
                    {
                        ParameterCount++;
                    }
                }
                else
                {
                    ParameterCount = GetParametersRecurse(
                        FunctionElement as List<object>
                        );
                }

            }
            return ParameterCount;
        }

    }
}