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
    /// Detailed information about a raycast.
    /// </summary>
    public class RayResult
    {
        /// <summary>
        /// The Enemy hit by a ray, null if ray didn't hit anything.
        /// </summary>
        public Enemy Hit;

        /// <summary>
        /// Position of the hit
        /// </summary>
        public Vector Position;

        /// <summary>
        /// Surface normal of the hit enemy
        /// </summary>
        public Vector Normal;

        public RayResult()
        {
        }
        public RayResult(Enemy hit, Vector pos, Vector norm)
        {
            Hit = hit;
            Position = pos;
            Normal = norm;
        }
    }

    /// <summary>
    /// Raycast is a Ray.
    /// A Ray is a half-line, that is finite in one direction, but infinite in the other. 
    /// It can be defined by a 2D point, where the line originates from, a direction vector, and length.
    /// </summary>
    public class Raycast
    {
        /// <summary>
        /// The Origin of the ray
        /// </summary>
        public Vector Origin = Vector.Zero;

        /// <summary>
        /// The direction of the ray (unit vector)
        /// </summary>
        public Vector Direction = Vector.Zero;

        /// <summary>
        /// The length of the ray (actual ray is Direction * Length)
        /// </summary>
        public double Length = 0.0;


        /// <summary>
        /// Return point on ray closest to point
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns></returns>
        public Vector ClosestPoint(Vector point)
        {
            Vector result = new Vector(point.X - Origin.X, point.Y - Origin.Y);
            double dirDist = Vector.DotProduct(result, Direction);

            if (dirDist < 0)
            {
                return result;
            }
            Console.WriteLine("Ray.ClosestPoint directionDistance was positive!");
            return Vector.Zero;
        }

        /// <summary>
        /// NOT DOCUMENTED YET
        /// </summary>
        /// <param name="point">NOT DOCUMENTED YET</param>
        /// <returns></returns>
        public double DistanceSqToPoint(Vector point)
        {
            Vector result = new Vector(point.X - Origin.X, point.Y - Origin.Y);
            double dirDist = Vector.DotProduct(result, Direction);

            // point behind the ray
            if (dirDist < 0)
            {
                return (Origin - point).Magnitude;
            }

            result = new Vector(Direction.X * dirDist, Direction.Y * dirDist);
            result += Origin;
            return (result - point).Magnitude;
        }

        /// <summary>
        /// NOT DOCUMENTED YET
        /// </summary>
        /// <param name="point">NOT DOCUMENTED YET</param>
        /// <returns></returns>
        public double DistanceToPoint(Vector point)
        {
            return Math.Sqrt(DistanceSqToPoint(point));
        }

        /// <summary>
        /// Return true if ray intersects an enemy
        /// </summary>
        /// <param name="part">The enemy</param>
        /// <returns></returns>
        public RayResult Intersects(Enemy part)
        {
            RayResult res = new RayResult();
            Vector clPos = ClosestPoint(part.Position);
            if (part.IsInside(clPos))
            {
                res.Hit = part;
                res.Position = clPos; //TODO: NOT RIGHT??
                return res;
            }
            return res;
        }

        public Raycast(Vector origin, Vector direction, double length = 1.0)
        {
            Origin = origin;
            Direction = direction;
            Length = length;
        }
    }
}
