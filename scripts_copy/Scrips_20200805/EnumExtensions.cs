using System.Collections.Concurrent;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
public static class EnumExtensions
{
    private static readonly ConcurrentDictionary<Type, Dictionary<long, string>> descDictionary
            = new ConcurrentDictionary<Type, Dictionary<long, string>>();
    /// <summary>
    /// 获取枚举对应的描述，该方法仅简单的获取描述信息，如未注册过描述信息则直接返回ToString()结果
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum input)
    {
        /*var enumType = input.GetType();
        if (enumType.IsEnum && descDictionary.TryGetValue(enumType, out Dictionary<long, string> dic) && dic != null)
        {
            var key = Convert.ToInt64(input);
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
        }
        return input?.ToString();*/
        return GetDescription<Attribute>(input);
    }
    /// <summary>
    /// 获取枚举对应的描述，该方法会在当前TEnum尚未注册描述信息时自动注册描述信息
    /// </summary>
    /// <typeparam name="TArrtibute">用于获取描述的Attribute</typeparam>
    /// <param name="input">枚举值</param>
    /// <param name="attrPropName">Attribute中用于获取描述值的public属性</param>
    /// <returns></returns>
    public static string GetDescription<TArrtibute>(this Enum input, string attrPropName = "Description")
        where TArrtibute : Attribute
    {
        var enumType = input.GetType();
        var attrType = typeof(TArrtibute);
        RegisterDescription(enumType, attrType, out Dictionary<long, string> dic, attrPropName);
        var key = Convert.ToInt64(input);
        if (dic != null && dic.Count > 0 && dic.ContainsKey(key))
        {
            return dic[key];
        }
        return input?.ToString();
    }
    /// <summary>
    /// 获取枚举所有的描述信息
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static IDictionary<TEnum, string> GetDescriptions<TEnum>()
        where TEnum : struct
    {
        var enumType = typeof(TEnum);
        if (enumType.IsEnum && descDictionary.TryGetValue(enumType, out Dictionary<long, string> dic) && dic != null)
        {
            return dic.ToDictionary(k=>((Enum.TryParse<TEnum>(k.Key.ToString(), true, out TEnum result))?result:result), v => v.Value);
        }
        return null;
    }
    /// <summary>
    /// 注册枚举的描述，假如已注册过属性描述信息，则该方法将不进行任何处理
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TArrtibute">用于获取描述的Attribute</typeparam>
    /// <param name="attrPropName">Attribute中用于获取描述值的public属性</param>
    public static void RegisterDescription<TEnum, TArrtibute>(string attrPropName = "Description")
        where TEnum : struct
        where TArrtibute : Attribute
    {
        var enumType = typeof(TEnum);
        var attrType = typeof(TArrtibute);
        RegisterDescription(enumType, attrType, out Dictionary<long, string> dictionary, attrPropName);
    }
    /// <summary>
    /// 注册枚举的描述，默认按<see cref="System.ComponentModel.DescriptionAttribute"/>进行注册
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public static void RegisterDescription<TEnum>()
        where TEnum : struct
    {
        RegisterDescription<TEnum, System.ComponentModel.DescriptionAttribute>();
    }
    /// <summary>
    /// 注册枚举的描述，注意此处的注册会替换已有的描述枚举（假如有）
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="dictionary">包含描述的字典</param>
    public static void RegisterDescription<TEnum>(IDictionary<TEnum, string> dictionary)
        where TEnum : struct
    {
        var enumType = typeof(TEnum);
        if (enumType.IsEnum)
        {
            var dic = dictionary.ToDictionary(k => Convert.ToInt64(k.Key), v => v.Value);
            descDictionary.AddOrUpdate(enumType, dic, (k, v) => dic);
        }
    }
    private static void RegisterDescription(Type enumType, Type attrType, out Dictionary<long, string> dictionary, string attrPropName)
    {
        dictionary = null;
        //仅枚举时才进行注册
        if (enumType.IsEnum
            && !descDictionary.TryGetValue(enumType, out dictionary))
        {
            var arrs = Enum.GetValues(enumType);
            dictionary = new Dictionary<long, string>();
            foreach (var e in arrs)
            {
                var desc = e.ToString();
                var attrs = enumType.GetField(desc).GetCustomAttributes(attrType, false);
                if (attrs.Length > 0)
                {
                    var prop = attrs[0].GetType().GetProperty(attrPropName);
                    if (prop != null)
                    {
                        desc = prop.GetValue(attrs[0]).ToString();
                    }
                }
                if (!dictionary.ContainsKey(Convert.ToInt64(e)))
                {
                    dictionary.Add(Convert.ToInt64(e), desc);
                }
            }
            descDictionary.TryAdd(enumType, dictionary);
        }
    }

}
