using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility
{
    public static class EnumHelper
    {
        public static string ToDescription(this System.Enum e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
            if (fieldInfo == null) return null;
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Any())
            {
                return attributes.FirstOrDefault().Description;
            }
            return null;
        }
    }
}
