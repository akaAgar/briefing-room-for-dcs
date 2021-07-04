using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Web.Utils
{
    public class Typeahead
    {

        public static Task<List<DatabaseEntryInfo>> SearchDB(DatabaseEntryType entryType, string searchText)
        {
            var list = BriefingRoom4DCS.BriefingRoom.GetDatabaseEntriesInfo(entryType);
            return Task.FromResult(list.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToList());
        }

        public static string GetDBDisplayName(DatabaseEntryType entryType, string id) {
            if(String.IsNullOrEmpty(id))
                return "Random";
            var list = BriefingRoom4DCS.BriefingRoom.GetDatabaseEntriesInfo(entryType);
            return BriefingRoom4DCS.BriefingRoom.GetDatabaseEntriesInfo(entryType).First(x => x.ID == id).Name;
        }

        public static string ConvertDB(DatabaseEntryInfo entry) => entry.ID;

        public static async Task<List<T>> SearchEnum<T>(string searchText) 
        {
            var list = new List<T>((T[])Enum.GetValues(typeof(T)));
            return await Task.FromResult(list.Where(x => x.ToString().ToLower().Contains(searchText.ToLower())).ToList());
        }
    }
}