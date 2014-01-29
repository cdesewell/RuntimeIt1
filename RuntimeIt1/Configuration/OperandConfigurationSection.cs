using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace RuntimeIt1.Configuration
{
    public class OperandConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("", IsDefaultCollection=true)]
        public OperandConfigurationCollection Operands
        {
            get
            {
                return (OperandConfigurationCollection)this[string.Empty];
            }
        }
    }
}