using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NBK.ECService.WMS.Utility
{
    public static class ExtendUtility
    {
        #region JTransform
        /// <summary>
        /// 自动建立sourceType-->destinationType 的映射 [包括内部类型的映射]
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        private static void JAutoCreateMap(Type sourceType, Type destinationType)
        {
            foreach (var perProperty in sourceType.GetProperties())
            {
                var dProperty = destinationType.GetProperty(perProperty.Name);
                if (dProperty != null)
                {
                    if (!perProperty.PropertyType.IsValueType && perProperty.PropertyType != typeof(string)) //非string的引用类型
                    {
                        if (perProperty.PropertyType.IsGenericType && perProperty.PropertyType.GenericTypeArguments.First().IsClass && perProperty.PropertyType.GenericTypeArguments.First() != typeof(string))
                        {
                            if (dProperty.PropertyType.IsGenericType && dProperty.PropertyType.GenericTypeArguments.First().IsClass && dProperty.PropertyType.GenericTypeArguments.First() != typeof(string))
                            {
                                JAutoCreateMap(perProperty.PropertyType.GenericTypeArguments.First(), dProperty.PropertyType.GenericTypeArguments.First());
                            }
                        }
                        else if (perProperty.PropertyType.IsClass && dProperty.PropertyType.IsClass) // 非泛型的属性,比如 MyType P3 {get;set;}
                        {
                            JAutoCreateMap(perProperty.PropertyType, dProperty.PropertyType);
                        }
                    }
                }
            }
            Mapper.CreateMap(sourceType, destinationType);
        }

        /// <summary>
        /// 转换到一个新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="mapRule">内部复杂属性的映射规则</param>
        /// <returns></returns>
        public static T JTransformTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            //Mapper.CreateMap(obj.GetType(), typeof(T));
            JAutoCreateMap(obj.GetType(), typeof(T));
            return Mapper.Map<T>(obj);
        }
        /// <summary>
        /// 集合转换到一个新的集合
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> JTransformTo<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                //Mapper.CreateMap(type, typeof(TDestination));
                JAutoCreateMap(type, typeof(TDestination));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 覆盖到destination 对象[即包括覆盖其中的引用类型属性中的值,但是不会覆盖引用类型属性的引用]
        /// [注意:其中引用类型属性的引用会不会被覆盖掉,但是引用类型中的值会被覆盖掉]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="nonCoveragePropertyNames">哪些属性不被覆盖"A1","A2.X","A3.Y.Z"</param>
        /// <returns></returns>
        public static object JCoverageTo(this object source, object destination, params string[] nonCoveragePropertyNames)
        {
            if (source == null) return destination;

            Type dt = destination.GetType();
            Type ot = source.GetType();

            foreach (var perProperty in ot.GetProperties())
            {
                var dProperty = dt.GetProperty(perProperty.Name);
                if (dProperty != null)
                {
                    if (nonCoveragePropertyNames != null && nonCoveragePropertyNames.Count() > 0 && dProperty.Name.IsIn(nonCoveragePropertyNames))
                    {
                        //在不被覆盖的集合中,则此属性不会覆盖
                        continue;
                    }

                    //非string的引用类型
                    if (!perProperty.PropertyType.IsValueType && perProperty.PropertyType != typeof(string))
                    {
                        //泛型
                        if (perProperty.PropertyType.IsGenericType && dProperty.PropertyType.IsGenericType)
                        {
                            //泛型参数 为引用
                            if (perProperty.PropertyType.GenericTypeArguments.First().IsClass && dProperty.PropertyType.GenericTypeArguments.First().IsClass)
                            {
                                //泛型参数 为非string的引用
                                if (perProperty.PropertyType.GenericTypeArguments.First() != typeof(string) && perProperty.PropertyType.GenericTypeArguments.First() != typeof(string))
                                {
                                    //集合形式的泛型属性,且泛型参数不是string,比如 List<MyType> P1 {get;set;}
                                    if (perProperty.PropertyType.GetInterface("IEnumerable", true) != null && dProperty.PropertyType.GetInterface("IEnumerable", true) != null)
                                    {
                                        var perPropertyValues = (IEnumerable)(perProperty.GetValue(source));
                                        var dPropertyValues = (IEnumerable)(dProperty.GetValue(destination));
                                        if (perPropertyValues != null && dPropertyValues != null)
                                        {
                                            var nonCoverageNamesTemp1 = nonCoveragePropertyNames.Where(p => p.StartsWith(dProperty.Name + "."));
                                            var ss = perPropertyValues.GetEnumerator();
                                            foreach (var dperValue in dPropertyValues)
                                            {
                                                if (ss.MoveNext())
                                                {
                                                    if (nonCoverageNamesTemp1 != null && nonCoverageNamesTemp1.Count() > 0)
                                                    {
                                                        ss.Current.JCoverageTo(dperValue, nonCoverageNamesTemp1.Select(p => p.Replace(dProperty.Name + ".", "")).ToArray());
                                                    }
                                                    else
                                                    {
                                                        ss.Current.JCoverageTo(dperValue);
                                                    }
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                    }
                                    else //非集合形式的泛型属性,且泛型参数不是string,比如 MyType1<MyType2> P11 {get;set;}
                                    {
                                        var nonCoverageNamesTemp2 = nonCoveragePropertyNames.Where(p => p.StartsWith(dProperty.Name + "."));
                                        if (nonCoverageNamesTemp2 != null && nonCoverageNamesTemp2.Count() > 0)
                                        {
                                            perProperty.GetValue(source).JCoverageTo(dProperty.GetValue(destination), nonCoverageNamesTemp2.Select(p => p.Replace(dProperty.Name + ".", "")).ToArray());
                                        }
                                        else
                                        {
                                            perProperty.GetValue(source).JCoverageTo(dProperty.GetValue(destination));
                                        }
                                    }
                                }
                                else //泛型参数为 string 的属性,比如 List<string> P1 {get;set;}  ,MyType1<string> P1 {get;set;}
                                {
                                    var pp = perProperty.GetValue(source);
                                    if (pp != null)
                                        dProperty.SetValue(destination, pp, null);
                                }
                            }
                            else //泛型参数为 值类型 的属性, List<int> P1 {get;set;} , MyType1<int> P1 {get;set;},
                            {
                                var tt = perProperty.GetValue(source);
                                if (tt != null)
                                    dProperty.SetValue(destination, tt, null);
                            }
                        }
                        else  //引用类型的属性,比如 MyType3 {get;set;}
                        {
                            var oo = perProperty.GetValue(source);
                            if (oo != null)
                                dProperty.SetValue(destination, oo, null);
                        }
                    }
                    else //值类型与string的属性
                    {
                        var hh = perProperty.GetValue(source);
                        if (hh != null)
                            dProperty.SetValue(destination, hh, null);
                    }

                }
            }

            return destination;
        }


        public static bool IsIn(this string thisValue, IEnumerable<string> collection)
        {
            return collection.Any(p => p == thisValue);
        }

        public static bool IsNotIn(this string thisValue, IEnumerable<string> collection)
        {
            return !thisValue.IsIn(collection);
        }

        #endregion

        #region JsonHelper

        /// <summary>
        /// 从一个对象信息生成Json串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonString(this object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //jss.MaxJsonLength = 999999999;   //如果数据量很大,这使用这个配置
            string jsonStr = jss.Serialize(obj);
            return jsonStr;
        }
        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object JsonToObject<T>(this string jsonString)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(jsonString);
        }

        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonToDto<T>(this string jsonString)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(jsonString);
        }

        #endregion

        /// <summary>
        /// 根据对象深度克隆  对象必须标记为      [Serializable]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            var type = typeof(T);

            if (!type.IsSerializable)
                return default(T);

            if (Object.ReferenceEquals(obj, null))
                return default(T);

            IFormatter format = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    format.Serialize(ms, obj);
                    ms.Seek(0, SeekOrigin.Begin);
                    return (T)format.Deserialize(ms);
                }
                catch (Exception e)
                {
                    return default(T);
                }
            }
        }

    }
}
