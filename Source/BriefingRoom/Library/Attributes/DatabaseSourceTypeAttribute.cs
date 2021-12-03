/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BriefingRoom4DCS
{
    public class DatabaseSourceTypeAttribute : ValidationAttribute
    {
        public DatabaseEntryType EntryType { get; }

        private readonly string[] AllowableValues;

        public DatabaseSourceTypeAttribute(DatabaseEntryType entryType, bool allowEmpty = false)
        {
            EntryType = entryType;

            List<string> allowableValuesList = new List<string>();
            if (allowEmpty) allowableValuesList.Add("");
            allowableValuesList.AddRange(BriefingRoom.GetDatabaseEntriesIDs(entryType));
            AllowableValues = allowableValuesList.ToArray();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues.Contains(value?.ToString()))
                return ValidationResult.Success;

            // TODO
            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }
}
