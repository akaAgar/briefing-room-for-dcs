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

namespace BriefingRoom
{
    /// <summary>
    /// Tacan Data object
    /// </summary>
    public class Tacan
    {
        /// <summary>
        /// Channel of TACAN.
        /// </summary>
        public readonly int channel;

        /// <summary>
        /// Mode of TACAN.
        /// </summary>
        public readonly char mode;

        /// <summary>
        /// Callsign of tacan.
        /// </summary>
        public readonly string callsign;

        /// <summary>
        /// Actual Radio Frequency getting this wrong means TACAN will appear on wrong channel.
        /// </summary>
        public readonly int freqency;

        public readonly bool AA;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Channel of TACAN.</param>
        /// <param name="callsign">Callsign of tacan</param>
        /// <param name="frequency">Frequency of tacan</param>
        public Tacan(int _channel, string _callsign, char _mode = 'X', bool AA = false)
        {
            channel = _channel;
            callsign = _callsign;
            mode = _mode;
            freqency = getTACANFrequency(_channel,_mode, AA);
        }

        //Pulled from calcTACANFrequencyMHz in DCS World OpenBeta\MissionEditor\modules\me_action_db.lua
        private int getTACANFrequency(int channel, char channelMode, bool AA)
        {
            int res;
            if 	(!AA && channelMode == 'X'){
                if(channel < 64)
                {
                    res =  962 + channel - 1;
                } else {
                    res =  1151 + channel - 64;
                }
            } else {
                if(channel < 64)
                {
                    res =  1088 + channel - 1;
                } else {
                    res =  1025 + channel - 64;
                }
            }

            return res * 1000000;
        }


        public override string ToString()
        {
            return $"{callsign}-{channel}{mode}";
        }
    }
}
