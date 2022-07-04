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
using System.Threading.Tasks;
using WkHtmlWrapper.Image.Converters;
using WkHtmlWrapper.Image.Options;

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

            var path = $"Flags\\{template.GetCoalitionID(template.ContextPlayerCoalition)}.png";
            if(File.Exists($"{BRPaths.INCLUDE_JPG}{path}"))
                imageLayers.Add(new ImageMakerLayer(path, ContentAlignment.TopLeft, 8, 8, 0, .5));

            byte[] imageBytes = imageMaker.GetImageBytes(imageLayers.ToArray());

            mission.AddMediaFile($"l10n/DEFAULT/title_{mission.UniqueID}.jpg", imageBytes);
        }

        internal static async Task GenerateKneeboardImagesAsync(DCSMission mission)
        {
            var html = mission.Briefing.GetBriefingKneeBoardTasksAndRemarksHTML();
            var inc = await GenerateKneeboardImageAsync(html, mission);
            html = mission.Briefing.GetBriefingKneeBoardFlightsHTML();
            inc = await GenerateKneeboardImageAsync(html, mission, inc);

            foreach (var flight in mission.Briefing.FlightBriefings)
            {
                html = flight.GetFlightBriefingKneeBoardHTML(mission);
                inc = await GenerateKneeboardImageAsync(html, mission, inc, flight.Type);
            }
        }

        private static async Task<int> GenerateKneeboardImageAsync(string html, DCSMission mission, int inc = 1, string aircraftID = "")
        {
            var converterlogs = "";
            try
            {
                string tempRenderPath = Path.ChangeExtension(Path.GetTempFileName(), ".png");
                converterlogs = await new HtmlToImageConverter().ConvertAsync(html, tempRenderPath, new GeneralImageOptions
                {
                    Width = 1200,
                    Transparent = true
                });
                var img = Image.FromFile(tempRenderPath);
                var pageCount = Math.Ceiling((decimal)(img.Size.Height / 1725.0));
                for (int i = 0; i < pageCount; i++)
                {
                    var page = new Bitmap(1200, 1800);
                    var graphics = Graphics.FromImage(page);
                    graphics.DrawImage(img, new Rectangle(0, 25, 1200, 1750), new Rectangle(0, i * 1725, 1200, 1750), GraphicsUnit.Pixel);
                    graphics.Dispose();
                    byte[] imageBytes;

                    ImageMaker imgMaker = new();
                    imgMaker.ImageSizeX = 1200;
                    imgMaker.ImageSizeY = 1800;

                    List<ImageMakerLayer> layers = new List<ImageMakerLayer>{
                        new ImageMakerLayer("notebook.png"),
                        new ImageMakerLayer(page)
                    };

                    var random = new Random();
                    if (random.Next(100) < 3)
                        layers.Add(easterEggLogos[random.Next(easterEggLogos.Count)]);

                    imageBytes = imgMaker.GetImageBytes(layers.ToArray());
                    var midPath = !string.IsNullOrEmpty(aircraftID) ? $"{aircraftID}/" : "";
                    mission.AddMediaFile($"KNEEBOARD/{midPath}IMAGES/comms_{mission.UniqueID}_{inc}.jpg", imageBytes);
                    inc++;
                }
                img.Dispose();
                File.Delete(tempRenderPath);
                return inc;

            }
            catch (System.Exception e)
            {
                throw new BriefingRoomException($"Failed to create KneeBoard Image converter logs {converterlogs}", e);    
            }

        }
    }
}