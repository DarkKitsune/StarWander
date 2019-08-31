using System;
using System.Collections.Generic;
using System.Text;

namespace StarWander
{
    public static class Conversion
    {
        public enum UnitType
        {
            EngineUnits,
            Kilometers,
            Meters,
            Centimeters
        }

        /// <summary>
        /// Convert to meters
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static decimal ToMeters(UnitType fromUnit, decimal value)
        {
            switch (fromUnit)
            {
                default:
                    throw new InvalidOperationException($"No case for unit type {fromUnit}");
                case UnitType.EngineUnits:
                case UnitType.Meters:
                    return value;
                case UnitType.Kilometers:
                    return value * 1000.0m;
                case UnitType.Centimeters:
                    return value / 1000.0m;
            }
        }

        /// <summary>
        /// Convert to engine units
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToEngineUnits(UnitType fromUnit, decimal value)
        {
            return ToMeters(fromUnit, value);
        }

        /// <summary>
        /// Convert from one unit to another
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Convert(UnitType fromUnit, UnitType toUnit, decimal value)
        {
            return ToMeters(fromUnit, value) / ToMeters(toUnit, 1.0m);
        }

        /// <summary>
        /// Convert a length to a simple human-readable string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToString(decimal units)
        {
            var kilometers = Convert(UnitType.EngineUnits, UnitType.Kilometers, units);
            if (kilometers >= 1m)
                return $"{(double)units:0.000} km";
            var meters = Convert(UnitType.EngineUnits, UnitType.Meters, units);
            return $"{(double)units:0.000} m";
        }
    }
}
