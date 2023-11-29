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
using IronPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BriefingRoom4DCS.Generator
{
    public class MissionGeneratorImages
    {

        internal static void GenerateTitleImage(DCSMission mission, MissionTemplateRecord template)
        {
            ImageMaker imageMaker = new();
            imageMaker.TextOverlay.Alignment = ContentAlignment.MiddleCenter;
            imageMaker.TextOverlay.Text = mission.Briefing.Name;

            List<ImageMakerLayer> imageLayers = new();
            string[] theaterImages = Directory.GetFiles(Path.Combine(BRPaths.INCLUDE_JPG, "Theaters"), $"{Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater).DCSID}*.jpg");
            if (theaterImages.Length == 0)
                imageLayers.Add(new ImageMakerLayer("_default.jpg"));
            else
                imageLayers.Add(new ImageMakerLayer(Path.Combine("Theaters", Path.GetFileName(Toolbox.RandomFrom(theaterImages)))));

            var path = Path.Combine("Flags", $"{template.GetCoalitionID(template.ContextPlayerCoalition)}.png");
            if (File.Exists(Path.Combine(BRPaths.INCLUDE_JPG, path)))
                imageLayers.Add(new ImageMakerLayer(path, ContentAlignment.TopLeft, 8, 8, 0, .5));

            byte[] imageBytes = imageMaker.GetImageBytes(imageLayers.ToArray());

            mission.AddMediaFile($"l10n/DEFAULT/title_{mission.UniqueID}.jpg", imageBytes);
        }

        internal static async Task GenerateKneeboardImagesAsync(DCSMission mission)
        {
            var html = mission.Briefing.GetBriefingKneeBoardTasksAndRemarksHTML();
            await GenerateKneeboardImageAsync(html, "Tasks", mission);

            html = mission.Briefing.GetBriefingKneeBoardFlightsHTML();
            await GenerateKneeboardImageAsync(html, "Flights", mission);

            html = mission.Briefing.GetBriefingKneeBoardGroundHTML();
            await GenerateKneeboardImageAsync(html, "Ground",  mission);

            foreach (var flight in mission.Briefing.FlightBriefings)
            {
                html = flight.GetFlightBriefingKneeBoardHTML();
                await GenerateKneeboardImageAsync(html, flight.Name, mission, flight.Type);
            }
        }



        public static async Task<List<byte[]>> GenerateKneeboardImageAsync(string html)
        {
            List<byte[]> output = new();
            try
            {
                var imagePaths = await GenerateImagePaths(html);
                foreach (var path in imagePaths)
                {
                    var img = Image.FromFile(path);
                    using var ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    output.Add(ms.ToArray());
                    img.Dispose();
                    File.Delete(path);
                }
                return output;

            }
            catch (Exception e)
            {
                throw new BriefingRoomException($"Failed to create KneeBoard Image", e);
            }

        }

        private static async Task<int> GenerateKneeboardImageAsync(string html, string name, DCSMission mission, string aircraftID = "")
        {
            try
            {
                var midPath = !string.IsNullOrEmpty(aircraftID) ? $"{aircraftID}/" : "";
                var imagePaths = await GenerateImagePaths(html);
                var multiImage = imagePaths.Count() > 1;
                var inc = 0;
                foreach (var path in imagePaths)
                {
                    var img = Image.FromFile(path);
                    using var ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    mission.AddMediaFile($"KNEEBOARD/{midPath}IMAGES/{name}{(multiImage? inc : "")}.png", ms.ToArray());
                    img.Dispose();
                    File.Delete(path);
                    inc++;
                }
                mission.AddMediaFile($"KNEEBOARD_HTML/{midPath}IMAGES/{name}.html", Encoding.UTF8.GetBytes(html));

                return inc;

            }
            catch (Exception e)
            {
                throw new BriefingRoomException($"Failed to create KneeBoard", e);
            }

        }

        private static async Task<string[]> GenerateImagePaths(string html) {
            var iWidth = 768;
            var iHeight = 1024;
            string tempRenderPath = Path.ChangeExtension(Path.GetTempFileName(), ".png").Replace(".png", "_*.png");
            ChromePdfRenderer renderer = new();
            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Custom;
            renderer.RenderingOptions.SetCustomPaperSizeinPixelsOrPoints(iWidth, iHeight);
            renderer.RenderingOptions.MarginTop = 0;
            renderer.RenderingOptions.MarginLeft = 0;
            renderer.RenderingOptions.MarginRight = 0;
            renderer.RenderingOptions.MarginBottom = 0;
            PdfDocument pdf = await renderer.RenderHtmlAsPdfAsync(html);
            var imagePaths = pdf.ToPngImages(tempRenderPath, iWidth, iHeight);

            return imagePaths;
        }
    }
}