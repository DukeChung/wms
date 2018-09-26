using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Configurations
{
    public class CustomConfigsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public CustomConfigElementCollection CustomConfigs
        {
            get
            {
                return (CustomConfigElementCollection)this[""];
            }
        }

    }

    public class CustomConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomConfigElement)element).Name;
        }

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
                return "cfg";
            }
        }
        public CustomConfigElement this[int index]
        {
            get
            {
                return (CustomConfigElement)BaseGet(index);
            }
        }
    }

    public class CustomConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("fliePath", IsRequired = true)]
        public string ConfigFliePath
        {
            get { return (string)this["fliePath"]; }
            set { this["fliePath"] = value; }
        }
    }
}
