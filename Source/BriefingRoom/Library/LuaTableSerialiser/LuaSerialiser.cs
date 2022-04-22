
using System.Collections.Generic;

namespace LuaTableSerialiser 
{
    public class LuaSerialiser
    {
        public static string Serialize(object data) => $"{Serialiser.ConvertType(data)}";
    }
}