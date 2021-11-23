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

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Enumerates all "families" of units, each consisting of a DCS World category (Plane, Vehicle...) and a role (Attack, Transport...)
    /// </summary>
    internal enum UnitFamily
    {
        /// <summary>
        /// Attack helicopters
        /// </summary>
        HelicopterAttack,
        /// <summary>
        /// Transport helicopter
        /// </summary>
        HelicopterTransport,
        /// <summary>
        /// Utility/light transport helicopter
        /// </summary>
        HelicopterUtility,

        /// <summary>
        /// Attack plane
        /// </summary>
        PlaneAttack,
        /// <summary>
        /// AWACS/early warning plane
        /// </summary>
        PlaneAWACS,
        /// <summary>
        /// Heavy bomber plane
        /// </summary>
        PlaneBomber,
        /// <summary>
        /// Drone
        /// </summary>
        PlaneDrone,
        /// <summary>
        /// Fighter plane
        /// </summary>
        PlaneFighter,
        /// <summary>
        /// Interceptor plane
        /// </summary>
        PlaneInterceptor,
        /// <summary>
        /// SEAD-capable plane
        /// </summary>
        PlaneSEAD,
        /// <summary>
        /// Strike plane
        /// </summary>
        PlaneStrike,
        /// <summary>
        /// Tanker plane (basket)
        /// </summary>
        PlaneTankerBasket,
        /// <summary>
        /// Tanker plane (boom)
        /// </summary>
        PlaneTankerBoom,
        /// <summary>
        /// Transport plane
        /// </summary>
        PlaneTransport,

        /// <summary>
        /// Aircraft carrier (CATOBAR)
        /// </summary>
        ShipCarrierCATOBAR,
        /// <summary>
        /// Aircraft carrier (STOBAR)
        /// </summary>
        ShipCarrierSTOBAR,
        /// <summary>
        /// Aircraft carrier (STOVL)
        /// </summary>
        ShipCarrierSTOVL,
        /// <summary>
        /// Cruiser
        /// </summary>
        ShipCruiser,
        /// <summary>
        /// Frigate
        /// </summary>
        ShipFrigate,
        /// <summary>
        /// Speedboat
        /// </summary>
        ShipSpeedboat,
        /// <summary>
        /// Submarine
        /// </summary>
        ShipSubmarine,
        /// <summary>
        /// Transport ship
        /// </summary>
        ShipTransport,

        /// <summary>
        /// Military structure
        /// </summary>
        StaticStructureMilitary,
        /// <summary>
        /// Production structure (civilian or military)
        /// </summary>
        StaticStructureProduction,

        StaticStructureOffshore,

        /// <summary>
        /// Mobile AAA
        /// </summary>
        VehicleAAA,
        /// <summary>
        /// Static AAA
        /// </summary>
        VehicleAAAStatic,
        /// <summary>
        /// APC/IFV
        /// </summary>
        VehicleAPC,
        /// <summary>
        /// Artillery
        /// </summary>
        VehicleArtillery,
        /// <summary>
        /// Infantry
        /// </summary>
        VehicleInfantry,
        /// <summary>
        /// Infantry with MANPADS
        /// </summary>
        VehicleInfantryMANPADS,
        /// <summary>
        /// Main battle tank
        /// </summary>
        VehicleMBT,
        /// <summary>
        /// Surface-to-surface missile
        /// </summary>
        VehicleMissile,
        /// <summary>
        /// Early-warning-radar
        /// </summary>
        VehicleEWR,
        /// <summary>
        /// SAM, long range
        /// </summary>
        VehicleSAMLong,
        /// <summary>
        /// SAM, medium range
        /// </summary>
        VehicleSAMMedium,
        /// <summary>
        /// SAM, short range
        /// </summary>
        VehicleSAMShort,
        /// <summary>
        /// SAM, short range, IR
        /// </summary>
        VehicleSAMShortIR,
        /// <summary>
        /// Transport vehicle
        /// </summary>
        VehicleTransport,
        /// <summary>
        /// FOB or FARP
        /// </summary>
        FOB
    }
}
