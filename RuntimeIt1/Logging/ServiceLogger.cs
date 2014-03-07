using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace RuntimeIt1.Logging
{
    public static class ServiceLogger
    {
        public static void LOG(string Host, string Method, string URL)
        {
            MySqlConnection LogConnection = new MySqlConnection("server=eu-cdbr-azure-west-b.cloudapp.net;" +
            "user=b0e5eeb68acc02;database=runtime;port=3306;password=8fa5cdf3;");
            LogConnection.Open();
            string LogQuery = "INSERT INTO logs (Host,Method,URL) VALUES ('" + Host + "','"+ Method +"','"+ URL +"')";
            MySqlCommand LogCommand = new MySqlCommand(LogQuery, LogConnection);
            LogCommand.ExecuteNonQuery();
            LogConnection.Close();
        }

    }
}