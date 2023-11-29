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
using System.Threading.Tasks;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorImages
    {

        internal static void GenerateTitle(DCSMission mission, MissionTemplateRecord template)
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
            var iWidth = 600;
            var iHeight = 863;
            try
            {
                string tempRenderPath = Path.ChangeExtension(Path.GetTempFileName(), ".png").Replace(".png", "_*.png");
                ChromePdfRenderer renderer = new();
                renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Custom;
                renderer.RenderingOptions.SetCustomPaperSizeinPixelsOrPoints(iWidth, iHeight);
                renderer.RenderingOptions.MarginTop = 0;
                renderer.RenderingOptions.MarginLeft = 0;
                renderer.RenderingOptions.MarginRight = 0;
                renderer.RenderingOptions.MarginBottom = 0;
                PdfDocument pdf = await renderer.RenderHtmlAsPdfAsync(html);
                // Console.WriteLine(html);
                var imagePaths = pdf.ToPngImages(tempRenderPath, iWidth, iHeight);


                foreach (var path in imagePaths)
                {
                    var img = Image.FromFile(path);
                    var midPath = !string.IsNullOrEmpty(aircraftID) ? $"{aircraftID}/" : "";
                    using var ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    mission.AddMediaFile($"KNEEBOARD/{midPath}IMAGES/comms_{mission.UniqueID}_{inc}.png", ms.ToArray());
                    img.Dispose();
                    File.Delete(path);
                    inc++;
                }

                return inc;

            }
            catch (System.Exception e)
            {
                throw new BriefingRoomException($"Failed to create KneeBoard Image converter logs {converterlogs}", e);
            }

        }
    }
}