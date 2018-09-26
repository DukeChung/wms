using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FortuneLab.Configurations
{
    /*

使用方法:
  
1. 在web.config 的<configSections> 节点中添加
<section name="customConfigs" type="FortuneLab.Core.Configurations.CustomConfigsSection,FortuneLab.Core"/>
     
2. 在web.config 中添加 如下配置
  <customConfigs>
    <cfg name="dbCommands" fliePath="ConfigFiles\dbCommands.config"/>
    <!--根据需要可以继续添加其他 cfg 配置,如
        <cfg name="dbConnections" fliePath="ConfigFiles\dbConnections.config"/>
        <cfg name="params" fliePath="ConfigFiles\params.config"/>
    -->
  </customConfigs>    
3. cs中通过 ConfigManager.Configs["dbCommands"] 获取
 例如 dbCommands 的配置文件内容如下
<?xml version="1.0" encoding="utf-8"?>
<Container>
<A A1="" A2="">
<A3><![CDATA[ xxxxxx ]]></A3>
<A4>
  <Parameters>
    <Param name="p1"></Param>
    <Param name="p2"></Param>
    <Param name="p3"></Param>
  </Parameters>
</A4>
</A>
</Container>
     
取A : ConfigManager.Configs["dbCommands"].A
取A1: ConfigManager.Configs["dbCommands"].A.A1
取A2: ConfigManager.Configs["dbCommands"].A.A2
取A3: ConfigManager.Configs["dbCommands"].A.A3
取A4: ConfigManager.Configs["dbCommands"].A.A4
取Parameters: ConfigManager.Configs["dbCommands"].A.Parameters
取Parameters 第一个: ConfigManager.Configs["dbCommands"].A.Parameters["p1"] 或者 ConfigManager.Configs["dbCommands"].A.Parameters[0]

终极用法: 可以将任意一个节点转换为IDictionary<string, dynamic>, 然后再操作
IDictionary<string, dynamic> dict = ConfigManager.Configs["dbCommands"].A as IDictionary<string, dynamic>;
取A1: dict["A1"]
取A2: dict["A2"]
取A3: dict["A3"]
取A4: dict["A4"]

IDictionary<string, dynamic> dict1 = dict["A4"] as IDictionary<string, dynamic>;
取Parameters: dict1["Parameters"]

IDictionary<string, dynamic> dict2 = dict1["Parameters"] as IDictionary<string, dynamic>;
取Parameters 第一个: dict2["p1"] 或 dict2[0]


备注: 
    任何地方都可以使用HasMember("memberName"), 取检查是否有这个成员
    可以用cfg_IsList="true" 来明确表明集合,集合中只有一项的时候,非常有用[否则就成对象与子对象的关系了]
    如果是集合, 可用使用foreach
    Value属性,可以取到尖括号之间的内容,比如<A>hahah</A> , 可以用A.Value 取到其中的值,所以请避免显示定义Value属性
*/
    /// <summary>
    /// XML配置文件管理
    /// Author: Jelex Wang
    /// </summary>
    public class ConfigManager
    {
        #region Create Config

        private static void CreateCustomConfig()
        {
            CustomConfigsSection config = (CustomConfigsSection)ConfigurationManager.GetSection("customConfigs");
            for (var i = 0; i < config.CustomConfigs.Count; i++)
            {
                var perConfig = config.CustomConfigs[i];
                var configFileFullPath = AppDomain.CurrentDomain.BaseDirectory + perConfig.ConfigFliePath;

                _allConfig.Add(perConfig.Name, CreateConfigDyObj(configFileFullPath));
                //_allConfigFile.Add(perConfig.Name, configFileFullPath);
                //MonitorConfigFile(configFileFullPath);
            }
        }
        private static dynamic CreateConfigDyObj(string configFileFullPath)
        {
            if (!File.Exists(configFileFullPath))
                throw new SystemException("未找到配置文件:" + configFileFullPath);

            XElement doc = XElement.Load(configFileFullPath);
            dynamic dyObj = CreateDyObj(doc);
            return dyObj;
        }

        private static dynamic CreateDyObj(XElement elem)
        {
            dynamic dyObj = new ExpandoObject();
            var dict = (IDictionary<string, dynamic>)dyObj;

            #region Elements of elem
            /*
            * 先检查是否有同名元素,
            *  如果有,则新建为XXX = new Dictionary<string,ExpandoObject>();
            *  如果没有,则新建为XXX = new ExpandoObject();
            * 
            */

            if (elem.HasElements)
            {
                var distinctNames = elem.Elements().Select(p => p.Name.ToString()).Distinct();
                if (distinctNames.Count() == elem.Elements().Count() && elem.Elements().Count() == 1) //只有一个子元素
                {
                    var cfg_IsList = elem.Attribute("cfg_IsList");
                    if (cfg_IsList != null && Convert.ToBoolean(cfg_IsList.Value)) //子元素按集合处理
                    {
                        WrapDictionary wrapDict = new WrapDictionary();
                        wrapDict.Add(elem.Elements().First().Attribute("name").Value, CreateDyObj(elem.Elements().First()));
                        dyObj = wrapDict;
                    }
                    else
                    {
                        dynamic oo = CreateDyObj(elem.Elements().First());
                        dict.Add(elem.Elements().First().Name.ToString(), oo);
                    }
                }
                else if (distinctNames.Count() == 1) //多个子元素,并且所有子元素都是 相同结构的
                {
                    if (dict.Keys.Count <= 0)
                    {
                        WrapDictionary wrapDict = new WrapDictionary();
                        foreach (var perElem in elem.Elements())
                        {
                            wrapDict.Add(perElem.Attribute("name").Value, CreateDyObj(perElem));
                        }

                        dyObj = wrapDict;
                    }
                }
                else if (distinctNames.Count() == elem.Elements().Count()) //多个子元素,所有子元素都是 不同结构的
                {
                    foreach (var perElem in elem.Elements())
                    {
                        dynamic oo = CreateDyObj(perElem);

                        dict.Add(perElem.Name.ToString(), oo);
                    }
                }
                else
                {
                    throw new Exception(string.Format("{0} subelement set error!", elem.Name.ToString()));
                }
            }
            #endregion

            #region Nodes of elem
            if (!elem.HasElements)
            {
                if (elem.Nodes().Count() > 0)
                {
                    XNode node = elem.Nodes().FirstOrDefault(p => p.NodeType != System.Xml.XmlNodeType.Comment);
                    if (node != null && (node.NodeType == System.Xml.XmlNodeType.CDATA || node.NodeType == System.Xml.XmlNodeType.Text))
                    {
                        dict.Add("Value", elem.Value);
                    }
                    else
                    {
                        dict.Add("Value", null);
                    }
                }
                else
                {
                    dict.Add("Value", null);
                }

                #region Attributes of elem
                if (elem.HasAttributes)
                {
                    foreach (var perAttr in elem.Attributes())
                    {
                        if (!dict.ContainsKey(perAttr.Name.ToString()))
                            dict.Add(perAttr.Name.ToString(), perAttr.Value);
                    }
                }
                #endregion
            }
            #endregion

            dict.Add("HasMember", new Func<string, bool>(memberName => dict.Keys.Contains(memberName)));

            return dyObj;
        }

        //private static void MonitorConfigFile(string configFileFullPath)
        //{
        //    var fodlerPath = configFileFullPath.Substring(0, configFileFullPath.LastIndexOf(@"\"));
        //    var fileName = configFileFullPath.Substring(configFileFullPath.LastIndexOf(@"\") + 1);

        //    FileSystemWatcher watcher = new FileSystemWatcher(fodlerPath, fileName);
        //    watcher.Changed += OnConfigFileChanged;
        //    watcher.EnableRaisingEvents = true;
        //}

        //private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        //{
        //    var tempKey = _allConfigFile.First(p => p.Value == e.FullPath).Key;
        //    dynamic newConfigObj = CreateConfigDyObj(e.FullPath);
        //    _allConfig[tempKey] = newConfigObj;
        //}

        #endregion

        //private static Dictionary<string, string> _allConfigFile = new Dictionary<string, string>(); //<configName,configFileFullPath>
        private static Dictionary<string, dynamic> _allConfig = new Dictionary<string, dynamic>();    //<configName,configObject>
        static object locker = new object();

        [Obsolete("此问题情况严重, 请使用ConfigManager.GetConfig做更新", true)]
        public static Dictionary<string, dynamic> Configs
        {
            get { return InternalConfigs; }
        }

        private static Dictionary<string, dynamic> InternalConfigs
        {
            get
            {
                if (_allConfig == null || _allConfig.Count < 1)
                {
                    lock (locker)
                    {
                        if (_allConfig == null || _allConfig.Count < 1)
                        {
                            ConfigManager.CreateCustomConfig();
                        }
                    }
                }
                return new Dictionary<string, dynamic>(_allConfig);
            }
        }

        public static dynamic GetConfig(string key)
        {
            if (InternalConfigs.ContainsKey(key))
                return InternalConfigs[key];
            throw new ConfigManagerException(key);
        }
    }

    public class ConfigManagerException : Exception
    {
        public ConfigManagerException(string key)
            : base($"ConfigManager获取{key}不存在, 请确认")
        {
        }
    }

    public class WrapDictionary : IEnumerable
    {
        ConcurrentDictionary<string, dynamic> dySet = new ConcurrentDictionary<string, dynamic>();

        public void Add(string memberName, dynamic obj)
        {
            dySet.TryAdd(memberName, obj);
        }
        public bool HasMember(string memberName)
        {
            return dySet.ContainsKey(memberName);
        }

        public dynamic this[string name]
        {
            get
            {
                //return dySet[name];
                return dySet.ContainsKey(name) ? dySet[name] : string.Empty;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in dySet)
            {
                yield return item.Value;
            }
        }
    }

}
