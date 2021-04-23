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

using System;
using BriefingRoom.Campaign;
using BriefingRoom.DB;
using BriefingRoom.Generator;
using BriefingRoom.Mission;
using BriefingRoom.Miz;
using BriefingRoom.Template;

namespace BriefingRoom
{
    /// <summary>
    /// Main class for the BriefingRoom library.
    /// </summary>
    public sealed class BriefingRoomLibrary : IDisposable
    {
        private readonly CampaignGenerator CampaignGen;
        private readonly Database Database;
        private readonly MissionGenerator Generator;

        public BriefingRoomLibrary()
        {
            Database = new Database();
            Generator = new MissionGenerator(Database);
            CampaignGen = new CampaignGenerator(Database, Generator);
        }

        public DCSMission GenerateMission(string templateFilePath)
        {
            return Generator.Generate(new MissionTemplate(templateFilePath));
        }

        public DCSMission GenerateMission(MissionTemplate template)
        {
            return Generator.Generate(template);
        }

        public MizFile MissionToMiz(DCSMission mission)
        {
            MizFile miz;

            using (MizMaker exporter = new MizMaker(Database))
                miz = exporter.ExportToMizFile(mission);

            return miz;
        }

        public void Dispose()
        {
            Generator.Dispose();
        }
    }
}
