using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using RuntimeIt1.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace RuntimeIt1.Controllers
{

    public class ProblemDictinary
    {
        public Dictionary<string, string> ProblemDirectory;
        public OperandConfigurationSection CoreOperands;
        public ProblemDictinary()
        {
            ProblemDirectory = new Dictionary<string, string>();
            CoreOperands = ConfigurationManager.GetSection("RuntimeCoreOperands") as OperandConfigurationSection;

            foreach (OperandConfigurationElement Operand in CoreOperands.Operands)
            {
                ProblemDirectory.Add(Operand.Name, Operand.Symbol);
            }

        }
        public void SetProblem(string ProblemKey, string ProblemValue)
        {
            ProblemDirectory.Add(ProblemKey, ProblemValue);
        }

        public string GetProblem(string ProblemKey)
        {
            foreach (KeyValuePair<string, string> Pair in ProblemDirectory)
            {
                if (Pair.Key == ProblemKey)
                {
                    return Pair.Value;
                }
            }
            return null;
        }

        public List<string> GetFunctions()
        {
            List<string> Functions = new List<string>();

            foreach (KeyValuePair<string, string> Pair in ProblemDirectory)
            {
                Functions.Add(Pair.Key);
            }
            return Functions;
        }

        


    }
}