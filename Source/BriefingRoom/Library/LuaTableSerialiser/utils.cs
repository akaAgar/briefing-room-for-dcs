using System;
using System.Collections;
using System.Collections.Generic;

namespace LuaTableSerialiser
{
    public class Utils
    {
        public static void PrintDict(Dictionary<object, object> dict, int nest = 0)
        {
            foreach (var item in dict)
            {
                if (item.Value is IDictionary)
                {
                    Console.WriteLine($"{GetNesting(nest)}K:{item.Key} V: ");
                    PrintDict((Dictionary<object, object>)item.Value, nest + 1);
                }
                else
                {
                    Console.WriteLine($"{GetNesting(nest)}K:{item.Key} V:{item.Value}");
                }
            }
        }

        public static string GetNesting(int nesting) => new string('\t', nesting);
    }
}