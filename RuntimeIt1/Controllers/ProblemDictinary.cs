using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using RuntimeIt1.Configuration;

namespace RuntimeIt1.Controllers
{
    
    public class ProblemDictinary
    {
        public Dictionary<string,object> ProblemDirectory;
        public List<object> Problem;
        public List<string> Solution;
        public OperandConfigurationSection CoreOperands;
        public ProblemDictinary()
        {
            ProblemDirectory = new Dictionary<string,object>();

            CoreOperands = ConfigurationManager.GetSection("RuntimeCoreOperands") as OperandConfigurationSection;

            foreach(OperandConfigurationElement Operand in CoreOperands.Operands)
            {
                ProblemDirectory.Add(Operand.Name,Operand.Symbol);
            }

        }        

        private List<string> GetProblemRecurse(List<object> ProblemList,List<string> Solution)
        {

            foreach(object ProblemElement in ProblemList)
            {
            if(ProblemElement is string)
            {
                Solution.Add(ProblemElement as string);
            }else
            {
                Solution = GetProblemRecurse(
                    ProblemElement as List<object>, Solution
                    );
            }
                
            }
            return Solution;
        }

        public List<string> GetProblem(string ProblemKey)
        {
            Solution = new List<string>();
            if (ProblemDirectory[ProblemKey] is string)
            {
                Solution.Add(ProblemDirectory[ProblemKey] as string);

            }
            else
            {
                Solution = GetProblemRecurse(
                    ProblemDirectory[ProblemKey] as List<object>,Solution
                    );
            }
            return Solution;
        }

        //private string 

        //public void setProblem()
        //{

        //}

        
    }
}