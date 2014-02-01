using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RuntimeIt1.Logging
{
    public static class ServiceLogger
    {

        public static void LOG(string Entry)
        {
            StreamWriter FileWriter;
            string FileName = "./LOGS/" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
            if (File.Exists(FileName))
            {
                FileWriter = File.AppendText(FileName);
                FileWriter.WriteLine(Entry);
                FileWriter.Close();
            }
            else
            {
                FileWriter = new StreamWriter(FileName);
                FileWriter.WriteLine(Entry);
                FileWriter.Close();
            }
        }
    }
}