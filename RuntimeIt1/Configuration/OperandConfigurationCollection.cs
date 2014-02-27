using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace RuntimeIt1.Configuration
{
    public class OperandConfigurationCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "Operand";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OperandConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OperandConfigurationElement)element).Name;
        }
    }
}