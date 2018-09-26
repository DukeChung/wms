using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FortuneLab.Utils
{
    public class XMLExtractor
    {
        public static string ExecuteXPathInFile(string fileName, string xpathExpression)
        {
            return ExecuteXPath(true, fileName, xpathExpression);
        }

        public static string ExecuteXPathInString(string fileName, string xpathExpression)
        {
            return ExecuteXPath(false, fileName, xpathExpression);
        }

        private static string ExecuteXPath(bool sourceIsFile, string stringOrFileName, string xpathExpression)
        {
            try
            {
                var doc = new XmlDocument();
                if (sourceIsFile)
                {
                    doc.Load(stringOrFileName);
                }
                else
                {
                    doc.LoadXml(stringOrFileName);
                }

                XmlNode node = doc.SelectSingleNode(xpathExpression);
                if (node != null && node.NodeType == XmlNodeType.Text)
                {
                    return node.InnerText;
                }
                if (node != null && (node.NodeType == XmlNodeType.CDATA || node.NodeType == XmlNodeType.Attribute))
                {
                    return node.InnerText;
                }
                if (node != null && node.NodeType == XmlNodeType.Element)
                {
                    return node.OuterXml;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
