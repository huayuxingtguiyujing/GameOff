using System.Collections.Generic;

namespace GameOff2023.Utils {

    /// <summary>
    /// 扩展字典类
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 可以直接通过给出key返回value,而不是像原方法一样返回bool值
        /// </summary>

        public static TValue TryGetValue<TKey,TValue>(this Dictionary<TKey,TValue> dict,TKey key)
        {
            TValue value;
            dict.TryGetValue(key, out value);

            return value;
        }

        public static Dictionary<TKey, TValue> UnionDic<TKey,TValue>(this Dictionary<TKey, TValue> dic1, Dictionary<TKey, TValue> dic2) {
            foreach (KeyValuePair<TKey, TValue> value in dic2)
            {
                dic1.Add(value.Key, value.Value);
            }
            return dic1;
        }

    } 

    /// <summary>
    /// 扩展String类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 判断文件名是否是Prefab文件
        /// </summary> 
        public static bool IsPrefabFile(this string str)
        {
            if(str.Length < 6) {
                return false;
            }
            //Debug.Log(str.Substring(str.Length - 6, 6));
            return str.Substring(str.Length - 6, 6).Equals("prefab");

        }
     
        public static bool IsValidDialogItem(this string str) {
            return str == null || str == "" || str == "None";
        }
    }
}
