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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Media;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorImages
    {

        // Just a bit of fun
        private static List<ImageMakerLayer> easterEggLogos = new List<ImageMakerLayer> {
            new ImageMakerLayer("icon.png", ContentAlignment.BottomRight, offsetX:-20, offsetY: -20, scale: 0.3),
            new ImageMakerLayer("razbari.png", ContentAlignment.BottomRight, offsetX:-20, offsetY: -20, scale: 0.1),
        };

        internal static void GenerateTitle(DCSMission mission, MissionTemplateRecord template)
        {
            ImageMaker imageMaker = new();
            imageMaker.TextOverlay.Alignment = ContentAlignment.MiddleCenter;
            imageMaker.TextOverlay.Text = mission.Briefing.Name;

            List<ImageMakerLayer> imageLayers = new List<ImageMakerLayer>();
            string[] theaterImages = Directory.GetFiles($"{BRPaths.INCLUDE_JPG}Theaters\\", $"{Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater).DCSID}*.jpg");
            if (theaterImages.Length == 0)
                imageLayers.Add(new ImageMakerLayer("_default.jpg"));
            else
                imageLayers.Add(new ImageMakerLayer("Theaters\\" + Path.GetFileName(Toolbox.RandomFrom(theaterImages))));

            imageLayers.Add(new ImageMakerLayer($"Flags\\{template.GetCoalitionID(template.ContextPlayerCoalition)}.png", ContentAlignment.TopLeft, 8, 8, 0, .5));

            byte[] imageBytes = imageMaker.GetImageBytes(imageLayers.ToArray());

            mission.AddMediaFile($"l10n/DEFAULT/title_{mission.UniqueID}.jpg", imageBytes);
        }

        internal static void GenerateKneeboardImage(DCSMission mission)
        {
            var text = mission.Briefing.GetBriefingKneeBoardText();
            var blocks = text.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var pages = new List<string>();
            var buildingPage = "";
            foreach (var block in blocks)
            {
                if (buildingPage.Count(f => f == '\n') + block.Count(f => f == '\n') > 32)
                {
                    pages.Add(buildingPage);
                    buildingPage = "";
                }
                buildingPage = $"{buildingPage}{block}\n\n";
            }
            if (!String.IsNullOrWhiteSpace(buildingPage))
                pages.Add(buildingPage);


            var inc = 1;
            foreach (var page in pages)
            {
                byte[] imageBytes;
                ImageMaker imgMaker = new();

                imgMaker.ImageSizeX = 800;
                imgMaker.ImageSizeY = 1200;
                imgMaker.TextOverlay.Shadow = false;
                imgMaker.TextOverlay.Color = Color.Black;
                imgMaker.TextOverlay.Text = $"{page}\n {inc}/{pages.Count()}";
                imgMaker.TextOverlay.FontSize = 14.0f;
                imgMaker.TextOverlay.FontFamily = "Arial";
                imgMaker.TextOverlay.Alignment = ContentAlignment.TopLeft;

                List<ImageMakerLayer> layers = new List<ImageMakerLayer>{
                        new ImageMakerLayer("notebook.png")
                    };

                var random = new Random();

                if (random.Next(100) < 3)
                    layers.Add(easterEggLogos[random.Next(easterEggLogos.Count)]);


                imageBytes = imgMaker.GetImageBytes(layers.ToArray());
                mission.AddMediaFile($"KNEEBOARD/IMAGES/comms_{mission.UniqueID}_{inc}.jpg", imageBytes);
                inc++;
            }
        }
    }
}