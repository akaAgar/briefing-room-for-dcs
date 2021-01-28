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

namespace BriefingRoom4DCSWorld
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Channel of TACAN.</param>
        /// <param name="callsign">Callsign of tacan</param>
        /// <param name="frequency">Frequency of tacan</param>
        public Tacan(int _channel, string _callsign, char _mode = 'X')
        {
            channel = _channel;
            callsign = _callsign;
            mode = _mode;
            freqency = getTACANFrequency(_channel,_mode);
        }

        private int getTACANFrequency(int channel, char channelMode)
        {
            int A = 1151;
            int B = 64;

            if (channel < 64)
                B = 1;

            if (channelMode == 'Y')
            {
                A = 1025;
                if (channel < 64)
                    A = 1088;
            }
            else
            {
                if (channel < 64)
                    A = 962;
            }

            return (A + channel - B) * 1000000;
        }


        public override string ToString()
        {
            return $"{callsign}-{channel}{mode}";
        }
    }
}
