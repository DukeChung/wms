using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class OBJTransformHelper
    {
        /// <summary>
        /// 自动建立sourceType-->destinationType 的映射
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="mapRule">内部复杂属性的映射规则</param>
        private static void AutoCreateMap(Type sourceType, Type destinationType, Func<Type, Type, bool> mapRule = null)
        {
            foreach (var perProperty in sourceType.GetProperties())
            {
                if (!perProperty.PropertyType.IsValueType && perProperty.PropertyType != typeof(string))
                {
                    if (
                            (perProperty.PropertyType.IsGenericType
                             && perProperty.PropertyType.GenericTypeArguments.First().IsClass
                             && !perProperty.PropertyType.GenericTypeArguments.First().IsValueType)
                        ||
                            (!perProperty.PropertyType.IsValueType && perProperty.PropertyType.IsClass)
                        )
                    {
                        Type ot = perProperty.PropertyType.GenericTypeArguments.First();
                        Type dt;
                        var dProperty = destinationType.GetProperties().FirstOrDefault(p =>
                        {
                            if (mapRule != null)
                            {
                                if (p.PropertyType.IsGenericType && mapRule(ot, p.PropertyType.GenericTypeArguments.First())) return true;
                                else if (mapRule(p.PropertyType, ot)) return true;
                                else return false;
                            }
                            else
                            {
                                if (p.PropertyType.IsGenericType && (ot.Name.TrimEnd("Dto".ToArray()) == p.PropertyType.GenericTypeArguments.First().Name)) return true;
                                else if (ot.Name.TrimEnd("Dto".ToArray()) == p.PropertyType.Name) return true;
                                else return false;
                            }

                        });
                        if (dProperty != null)
                        {
                            if (dProperty.PropertyType.IsGenericType) dt = dProperty.PropertyType.GenericTypeArguments.First();
                            else dt = dProperty.PropertyType;

                            AutoCreateMap(ot, dt, mapRule);
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
        public static T TransformTo<T>(this object obj, Func<Type, Type, bool> mapRule = null)
        {
            if (obj == null) return default(T);
            AutoCreateMap(obj.GetType(), typeof(T), mapRule);
            //Mapper.CreateMap(obj.GetType(), typeof(T));
            return Mapper.Map<T>(obj);
        }
        /// <summary>
        /// 集合转换到一个新的集合
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapRule">内部复杂属性的映射规则</param>
        /// <returns></returns>
        public static List<TDestination> TransformTo<TDestination>(this IEnumerable source, Func<Type, Type, bool> mapRule = null)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                AutoCreateMap(type, typeof(TDestination), mapRule);
                //Mapper.CreateMap(type, typeof(TDestination));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 集合转换到一个新的集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapRule">内部复杂属性的映射规则</param>
        /// <returns></returns>
        public static List<TDestination> TransformTo<TSource, TDestination>(this IEnumerable<TSource> source, Func<Type, Type, bool> mapRule = null)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            AutoCreateMap(typeof(TSource), typeof(TDestination), mapRule);
            //Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 覆盖到destination 对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="mapRule">内部复杂属性的映射规则</param>
        /// <returns></returns>
        public static TDestination TransformTo<TSource, TDestination>(this TSource source, TDestination destination, Func<Type, Type, bool> mapRule = null)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            AutoCreateMap(typeof(TSource), typeof(TDestination), mapRule);
            //Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map(source, destination);
        }
    }
}
