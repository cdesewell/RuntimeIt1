using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using RuntimeIt1.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using RuntimeIt1.Exceptions;
using System.Net;

namespace RuntimeIt1.Controllers
{

    public class ProblemDictinary
    {
        public Dictionary<string, string> ProblemDirectory;
        public ProblemDictinary(OperandConfigurationSection CoreOperands)
        {
            ProblemDirectory = new Dictionary<string, string>();

            foreach (OperandConfigurationElement Operand in CoreOperands.Operands)
            {
                ProblemDirectory.Add(Operand.Name, Operand.Symbol);
            }

        }
        public void SetProblem(string ProblemKey, string ProblemValue)
        {
            if (ProblemDirectory.ContainsKey(ProblemKey))
            {
                throw new ServiceException(HttpStatusCode.Forbidden, "Problem already defined");
            }
            ProblemDirectory.Add(ProblemKey, ProblemValue);
        }

        public void UpdateProblem(string ProblemKey, string ProblemValue)
        {

            if (ProblemDirectory.ContainsKey(ProblemKey))
            {
                ProblemDirectory[ProblemKey] = ProblemValue;
            }
            else
            {
                throw new ServiceException(HttpStatusCode.NotFound, "Problem not found");
            }

        }

        public string GetProblem(string ProblemKey)
        {
            try
            {
                return ProblemDirectory[ProblemKey];
            }
            catch (KeyNotFoundException)
            {
                throw new ServiceException(HttpStatusCode.NotFound, "Problem not found");
            }
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

        public void DeleteProblem(string ProblemKey)
        {
            try
            {
                ProblemDirectory.Remove(ProblemKey);
            }
            catch (KeyNotFoundException)
            {
                throw new ServiceException(HttpStatusCode.NotFound, "Problem not found");
            }

        }
    }
}