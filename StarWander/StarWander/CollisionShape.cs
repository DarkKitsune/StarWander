using System;
using VulpineLib.Geometry;
using VulpineLib.Util;
using StarWander.Components;

namespace StarWander
{
    public class CollisionShape
    {
        /// <summary>
        /// Sphere shape
        /// </summary>
        public Sphere Sphere { get; }

        /// <summary>
        /// The hull the shape belongs to, if any
        /// </summary>
        public CollisionHull? CollisionHull;

        public CollisionShape(Vector3<float> position, float radius)
        {

        }

        /// <summary>
        /// Check if colliding with another collision shape
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCollidingWith(CollisionShape other)
        {
            var sphere1 = Sphere;
            var sphere2 = other.Sphere;
            if (!(CollisionHull is null))
                sphere1.Center += CollisionHull.Parent.Transform.GetPositionInRegion(CollisionHull!.Parent.Region).To<float>();
            if (!(other.CollisionHull is null))
                sphere2.Center += other.CollisionHull.Parent.Transform.GetPositionInRegion(CollisionHull!.Parent.Region).To<float>();
            return sphere1.Intersects(sphere2);
        }
    }
}
