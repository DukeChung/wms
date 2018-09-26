using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FortuneLab.ErrorCodes
{
    [XmlRoot("ErrorCode")]
    public class ErrorCode
    {
        [XmlArray("Messages")]
        [XmlArrayItem("Message")]
        public List<ErrorMessage> ErrorCodeList { get; set; }
    }

    public class ErrorMessage
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("msg")]
        public string Msg { get; set; }

    }

    [Obsolete("请使用ErrorMessage替换", true)]
    public class Message : ErrorMessage
    {

    }
}