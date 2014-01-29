using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace RuntimeIt1.Configuration
{
    public class OperandConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Name",IsKey=true)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
        }
        [ConfigurationProperty("Symbol")]
        public string Symbol
        {
            get
            {
                return (string)this["Symbol"];
            }
        }
    }
}