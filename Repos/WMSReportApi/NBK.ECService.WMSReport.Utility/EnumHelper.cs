using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NBK.ECService.WMSReport.Utility
{
    public static class EnumHelper
    {

        public static string ToDescription(this System.Enum e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Any())
            {
                return attributes.FirstOrDefault().Description;
            }
            return null;
        }
    }
}