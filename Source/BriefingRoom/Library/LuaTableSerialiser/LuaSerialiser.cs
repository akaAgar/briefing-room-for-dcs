
using System.Collections.Generic;

namespace LuaTableSerialiser
{
    public class LuaSerialiser
    {
        public static string Serialize(object data) => $"{Serialiser.ConvertType(data)}";

        public static Dictionary<object, object> Deserialize(string data) => Deserializer.ToDict(data);
    }
}