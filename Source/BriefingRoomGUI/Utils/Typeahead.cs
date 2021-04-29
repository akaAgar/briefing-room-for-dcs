using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS;
using BriefingRoom4DCS.Data;

namespace BriefingRoomGUI.Utils
{
    public class Typeahead
    {

        public static Task<List<T>> SearchDB<T>(string searchText) where T: DBEntry
        {
            var list = Database.Instance.GetAllEntries<T>().ToList();
            return Task.FromResult(list.Where(x => x.UIDisplayName.ToLower().Contains(searchText.ToLower())).ToList());
        }

        public static string GetDBDisplayName<T>(string id) where T: DBEntry {
            if(String.IsNullOrEmpty(id))
                return "Random";
            return Database.Instance.GetEntry<T>(id).UIDisplayName;
        }

        public static string? ConvertDB(DBEntry entry) => entry?.ID;

        public static async Task<IEnumerable<T>> SearchEnum<T>(string searchText) 
        {
            var list = new List<T>((T[])Enum.GetValues(typeof(T)));
            return await Task.FromResult(list.Where(x => x.ToString().ToLower().Contains(searchText.ToLower())).ToList());
        }
    }
}