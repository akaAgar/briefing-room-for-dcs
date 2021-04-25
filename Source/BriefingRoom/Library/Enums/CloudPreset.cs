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


using System.ComponentModel.DataAnnotations;

namespace BriefingRoom4DCS
{
    public enum CloudPreset
    {
        [Display(Name = "Random", Description = "Use Random Cloud Preset")]
        Random,

        [Display(Name = "None", Description = "Use No cloud Preset.")]
        None,

        [Display(Name = "Light Scattered 1", Description = "Few Scattered Clouds \nMETAR:FEW/SCT 7/8.")]
        Preset1,

        [Display(Name = "Light Scattered 2", Description = "Two Layers Few and Scattered \nMETAR:FEW/SCT 8/10 SCT 23/24")]
        Preset2,

        [Display(Name = "High Scattered 1", Description = "Two Layer Scattered \nMETAR:SCT 8/9 FEW 21")]
        Preset3,

        [Display(Name = "High Scattered 2", Description = "Two Layer Scattered \nMETAR:SCT 8/10 FEW/SCT 24/26")]
        Preset4,

        [Display(Name = "High Scattered 3", Description = "Two Layer Scattered/Broken High Altitude \nMETAR:SCT/BKN 18/20 FEW 36/38 FEW 40")]
        Preset8,

        [Display(Name = "Scattered 1", Description = "Three Layer High altitude Scattered \nMETAR:SCT 14/17 FEW 27/29 BKN 40")]
        Preset5,

        [Display(Name = "Scattered 2", Description = "One Layer Scattered/Broken \nMETAR:SCT/BKN 8/10 FEW 40")]
        Preset6,

        [Display(Name = "Scattered 3", Description = "Two Layer Scattered/Broken \nMETAR:BKN 7.5/12 SCT/BKN 21/23 SCT 40")]
        Preset7,

        [Display(Name = "Scattered 4", Description = "Two Layer Broken/Scattered \nMETAR:BKN 7.5/10 SCT 20/22 FEW41")]
        Preset9,

        [Display(Name = "Scattered 5", Description = "Two Layers Scattered Large Thick Clouds  \nMETAR:SCT/BKN 18/20 FEW36/38 FEW 40")]
        Preset10,

        [Display(Name = "Scattered 6", Description = "Two Layers Scattered Large Clouds High Ceiling \nMETAR:BKN 18/20 BKN 32/33 FEW 41")]
        Preset11,

        [Display(Name = "Scattered 7", Description = "Two Layers Scattered Large Clouds High Ceiling \nMETAR:BKN 12/14 SCT 22/23 FEW 41")]
        Preset12,

        [Display(Name = "Broken 1", Description = "Two Layers Broken Clouds \nMETAR:BKN 12/14 BKN 26/28 FEW 41")]
        Preset13,

        [Display(Name = "Broken 2", Description = "Broken Thick Low Layer with Few High Layer\nMETAR:BKN LYR 7/16 FEW 41")]
        Preset14,

        [Display(Name = "Broken 3", Description = "Two Layers Broken Large Clouds \nMETAR:SCT/BKN 14/18 BKN 24/27 FEW 40")]
        Preset15,

        [Display(Name = "Broken 5", Description = "Three Layers Broken/Overcast \nMETAR:BKN/OVC LYR 7/13 20/22 32/34")]
        Preset17,

        [Display(Name = "Broken 6", Description = "Three Layers Broken/Overcast \nMETAR:BKN/OVC LYR 13/15 25/29 38/41")]
        Preset18,

        [Display(Name = "Broken 7", Description = "Three Layers Overcast At Low Level \nMETAR:OVC 9/16 BKN/OVC LYR 23/24 31/33")]
        Preset19,

        [Display(Name = "Broken 8", Description = "Three Layers Overcast Low Level \nMETAR:BKN/OVC 13/18 BKN 28/30 SCT FEW 38")]
        Preset20,

        [Display(Name = "Overcast 1", Description = "Overcast low level \nMETAR:BKN/OVC LYR 7/8 17/19")]
        Preset21,

        [Display(Name = "Overcast 2", Description = "Overcast low Level \nMETAR:BKN LYR 7/10 17/20")]
        Preset22,

        [Display(Name = "Overcast 3", Description = "Three Layer Broken Low Level Scattered High \nMETAR:BKN LYR 11/14 18/25 SCT 32/35")]
        Preset23,

        [Display(Name = "Overcast 4", Description = "Three Layer Overcast \nMETAR:BKN/OVC 3/7 17/22 BKN 34")]
        Preset24,

        [Display(Name = "Overcast 5", Description = "Three Layer Overcast \nMETAR:OVC LYR 12/14 22/25 40/42")]
        Preset25,

        [Display(Name = "Overcast 6", Description = "Three Layer Overcast \nMETAR:OVC 9/15 BKN 23/25 SCT 32")]
        Preset26,

        [Display(Name = "Overcast 7", Description = "Three Layer Overcast \nMETAR:OVC 8/15 SCT/BKN 25/26 34/36")]
        Preset27,

        [Display(Name = "Overcast And Rain 1", Description = "Overcast with Rain \nMETAR:VIS 3-5KM RA OVC 3/15 28/30 FEW 40")]
        RainyPreset1,

        [Display(Name = "Overcast And Rain 2", Description = "Overcast with Rain \nMETAR:VIS 1-5KM RA BKN/OVC 3/11 SCT 18/29 FEW 40")]
        RainyPreset2,

        [Display(Name = "Overcast And Rain 3", Description = "Overcast with Rain \nMETAR:VIS 3-5KM RA OVC LYR 6/18 19/21 SCT 34")]
        RainyPreset3
    }
}