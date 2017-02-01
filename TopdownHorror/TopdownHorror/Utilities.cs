using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

namespace TopdownHorror
{
    /// <summary>
    /// Contains all directions of travel and their angle (North, Southwest, etc.)
    /// </summary>
    public enum CompassDirection
    {
        North = 90,
        Northeast = 45,
        East = 0,
        Southeast = -45,
        South = -90,
        Southwest = -135,
        West = 180,
        Northwest = 135
    }

    /// <summary>
    /// A Container for some useful utilities
    /// </summary>

    static class Utilities
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        /// <summary>
        /// Transforms corresponding elements of the polar coordinate arrays theta and rho to two-dimensional Cartesian, or xy, coordinates.
        /// </summary>
        /// <param name="theta">Theta in degrees</param>
        /// <param name="rho">Radial coordinate</param>
        /// <returns>Cartesian vector</returns>
        public static Vector PolarToCartesian(double theta, double rho = 1.0)
        {
            theta = DegreeToRadian(theta);
            return new Vector(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }

        /// <summary>
        /// Contains all compass direction angles and their unit vectors.
        /// </summary>
        public static Dictionary<Vector, double> UnitVectorAngles = new Dictionary<Vector, double>()
            {
                {new Vector(0, 0), 90.0},
                {new Vector(0, 1), 90.0},
                {new Vector(0, -1), -90.0},
                {new Vector(1, 0), 0.0},
                {new Vector(-1, 0), 180.0},
                {new Vector(1, 1), 45.0},
                {new Vector(1, -1), -45.0},
                {new Vector(-1, 1), 135.0},
                {new Vector(-1, -1), -135.0},
            };

        /// <summary>
        /// Contains all compass directions and their unit vectors.
        /// </summary>
        public static Dictionary<Vector, CompassDirection> UnitVectorDirections = new Dictionary<Vector, CompassDirection>()
            {
                {new Vector(0, 1), CompassDirection.North},
                {new Vector(0, -1), CompassDirection.South},
                {new Vector(1, 0), CompassDirection.East},
                {new Vector(-1, 0), CompassDirection.West},
                {new Vector(1, 1), CompassDirection.Northeast},
                {new Vector(1, -1), CompassDirection.Southeast},
                {new Vector(-1, 1), CompassDirection.Northwest},
                {new Vector(-1, -1), CompassDirection.Southwest},
            };

        /// <summary>
        /// Returns compass direction (North, Southwest, etc.) from a given direction unit vector. 
        /// Returns null if given Vector is not a unit vector.
        /// </summary>
        /// <param name="vec">Direction unit vector</param>
        /// <returns></returns>
        public static CompassDirection? GetCompassDirectionFromUnitVector(Vector vec)
        {
            if (UnitVectorDirections.ContainsKey(vec))
            {
                return UnitVectorDirections[vec];
            }
            return null;
        }


        /// <summary>
        /// Returns an Angle from a given direction unit vector.
        /// Returns null if given Vector is not a unit vector.
        /// </summary>
        /// <param name="dir">Direction unit vector</param>
        /// <returns></returns>
        public static Angle GetAngleFromDirection(Vector dir)
        {
            if (UnitVectorAngles.ContainsKey(dir))
            {
                return Angle.FromDegrees(UnitVectorAngles[dir]);
            }
            return Angle.Zero;
        }

        /// <summary>
        /// Returns the closest enemy to point in an enemy list
        /// </summary>
        /// <param name="enemies">List of enemies</param>
        /// <param name="point">2D Point</param>
        /// <returns></returns>
        public static Enemy ClosestEnemy(List<Enemy> enemies, Vector point)
        {
            Enemy closest = enemies[0];
            for (int i = 1; i < enemies.Count; i++ )
            {
                Enemy current = enemies[i];
                double oldDist = (closest.Position - point).Magnitude;
                double newDist = (current.Position - point).Magnitude;
                if (newDist < oldDist)
                {
                    closest = current;
                }
            }
            return closest;
        }

        /// <summary>
        /// Returns value clamped between min and max values.
        /// </summary>
        /// <param name="val">Value input</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
