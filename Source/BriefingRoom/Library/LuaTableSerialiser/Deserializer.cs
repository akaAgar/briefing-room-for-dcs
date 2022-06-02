using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LuaTableSerialiser
{
    public class Deserializer
    {
        internal static Dictionary<object, object> ToDict(string data)
        {
            var keyReplace = Regex.Replace(data, "\\[(.+?)\\] = ", "$1:");
            var digitFix = Regex.Replace(keyReplace, "(\\d+):", "\"~$1\":");
            var removeSpace = Regex.Replace(digitFix, ",(\\s*})", "$1");
            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(removeSpace);
            return ConvertTypes(deserializedData);
        }

        private static Dictionary<object, object> ConvertTypes(Dictionary<string, object> data)
        {
            var dict = new Dictionary<object, object>();
            foreach (var x in data)
            {
                dict[ConvertKey(x.Key)] = x.Value is JObject @object ? ConvertTypes(@object.ToObject<Dictionary<string, object>>()) : x.Value;
            }
            return dict;
        }

        private static object ConvertKey(string key)
        {
            if (!key.StartsWith("~"))
                return key;
            key = key.Replace("~", "");
            if (key.Contains("."))
                return float.Parse(key);
            return int.Parse(key);
        }
    }
}