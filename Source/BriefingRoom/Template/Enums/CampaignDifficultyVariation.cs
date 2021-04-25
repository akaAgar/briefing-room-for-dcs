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

using System.ComponentModel.DataAnnotations;

namespace BriefingRoom.Campaign
{
    /// <summary>
    /// Enumerates various possible difficulty variations during the progress of a campaign.
    /// </summary>
    public enum CampaignDifficultyVariation
    {
        [Display(Name = "Random", Description = "Pick a difficulty variation randomly.")]
        Random,

        [Display(Name = "Considerably easier", Description = "Campaign become a lot easier as the player completes missions.")]
        ConsiderablyEasier,

        [Display(Name = "Much easier", Description = "Campaign become easier as the player completes missions.")]
        MuchEasier,

        [Display(Name = "Somewhat easier", Description = "Campaign become somewhat easier as the player completes missions.")]
        SomewhatEasier,

        [Display(Name = "Steady", Description = "Campaign difficulty stays the same during all missions.")]
        Steady,

        [Display(Name = "Somewhat harder", Description = "Campaign become somewhat harder as the player completes missions.")]
        SomewhatHarder,

        [Display(Name = "Much harder", Description = "Campaign become harder as the player completes missions.")]
        MuchHarder,

        [Display(Name = "Considerably harder", Description = "Campaign become a lot harder as the player completes missions.")]
        ConsiderablyHarder
    }
}
