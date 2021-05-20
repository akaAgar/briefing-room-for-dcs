/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores settings (number of units, etc.) about a level of CAP.
    /// </summary>
    internal struct DBCommonCAPLevel
    {
        /// <summary>
        /// Possible AI skill levels for units at this CAP level.
        /// </summary>
        internal DCSSkillLevel[] SkillLevel { get; }

        /// <summary>
        /// Number of CAP aircraft (not flight groups) to spawn at this CAP level.
        /// </summary>
        internal MinMaxI UnitCount { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">.ini file from which to load air defense common settings</param>
        /// <param name="capLevel">Level of CAP for which this setting applies.</param>
        internal DBCommonCAPLevel(INIFile ini, AmountNR capLevel)
        {
            if ((capLevel == AmountNR.None) || (capLevel == AmountNR.Random))
            {
                SkillLevel = new DCSSkillLevel[] { DCSSkillLevel.Average };
                UnitCount = new MinMaxI(0, 0);
                return;
            }

            SkillLevel = ini.GetValueArray<DCSSkillLevel>("CAPLevels", $"{capLevel}.SkillLevel").Distinct().ToArray();
            if (SkillLevel.Length == 0) SkillLevel = new DCSSkillLevel[] { DCSSkillLevel.Average, DCSSkillLevel.Good, DCSSkillLevel.High, DCSSkillLevel.Excellent };
            UnitCount = ini.GetValue<MinMaxI>("CAPLevels", $"{capLevel}.UnitCount");
        }
    }
}
