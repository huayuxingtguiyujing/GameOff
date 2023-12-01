using System.Collections.Generic;

namespace GameOff2023.Utils {

    /// <summary>
    /// ��չ�ֵ���
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// ����ֱ��ͨ������key����value,��������ԭ����һ������boolֵ
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
    /// ��չString��
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// �ж��ļ����Ƿ���Prefab�ļ�
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
