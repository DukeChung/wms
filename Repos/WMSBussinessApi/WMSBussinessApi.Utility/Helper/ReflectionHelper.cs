using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WMSBussinessApi.Utility.Helper
{
    public class ReflectionHelper
    {
        /// <summary>
        /// 获取所有成员
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MemberInfo[] GetAllMembers(Type t)
        {
            return t.GetMembers();
        }

        /// <summary>
        /// 获取单个成员
        /// </summary>
        /// <param name="t"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        private static MemberInfo GetOneMember(Type t, string memberName)
        {
            return GetAllMembers(t).FirstOrDefault(m => m.Name == memberName);
        }

        /// <summary>
        /// 获取所有的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties(object obj)
        {
            var type = obj.GetType();
            return type.GetProperties();
        }

        /// <summary>
        /// 获取成员的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(object obj, string propertyName)
        {
            var type = obj.GetType(); 
            var member = GetOneMember(type, propertyName);
            return type.GetProperty(member.Name);
        }

        /// <summary>
        /// 获取方法的返回值
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="instance"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object GetMethodValue(string methodName, object instance, params object[] param)
        {
            Type t = instance.GetType();
            MethodInfo info = t.GetMethod(methodName);
            return info.Invoke(instance, param);
        }

        /// <summary>
        /// 获取属性的类型
        /// 说明：null表示没有找到
        /// </summary>
        /// <param name="propertyName">成员的名称</param>
        /// <param name="t">所在类的类型</param>
        /// <returns></returns>
        public static string GetPropertyType(string propertyName, Type t)
        {
            MemberInfo member = GetOneMember(t, propertyName);
            if (member != null)
            {
                PropertyInfo property = t.GetProperty(member.Name);
                return property.PropertyType.Name;
            }
            return null;

        }

        /// <summary>
        /// 获取单个成员是否含有某个特性
        /// </summary>
        /// <param name="memberName">成员的名称</param>
        /// <param name="t">所在类的类型</param>
        /// <param name="attribute">要获取的特性</param>
        /// <returns></returns>
        public static bool CustomAttributeExist(string memberName, Type t, Attribute attribute)
        {

            var member = GetOneMember(t, memberName);
            var my_customAttribute = member.CustomAttributes.FirstOrDefault(a => a.AttributeType == attribute.GetType());
            return my_customAttribute != null;
        }

        /// <summary>
        /// 给成员设值
        /// </summary>
        /// <param name="obj">目标类</param>
        /// <param name="memberName">类内属性名称</param>
        /// <param name="value">设置的值</param>
        public static void SetValue(object obj, string memberName, object value)
        {
            var property = GetProperty(obj, memberName);
            property.SetValue(obj, value);
        }

        /// <summary>
        /// 获取成员的值
        /// </summary>
        /// <param name="obj">目标类</param>
        /// <param name="memberName">成员的名称</param>
        /// <returns></returns>
        public static object GetValue(object obj, string memberName)
        {
            var property = GetProperty(obj, memberName);
            return property.GetValue(obj);
        }
    }
}
