using System;
using VulpineLib.Util;

namespace StarWander.Components
{
    public class BoundingBox : Component
    {
        /// <summary>
        /// Minimum corner of the bounding box in local coordinates
        /// </summary>
        public virtual Vector3<float> MinLocal { get; private set; }

        /// <summary>
        /// Maximum corner of the bounding box in local coordinates
        /// </summary>
        public virtual Vector3<float> MaxLocal { get; private set; }

        /// <summary>
        /// Center of the bounding box in local coordinates
        /// </summary>
        public Vector3<float> CenterLocal => (MinLocal + MaxLocal) / 2f;

        /// <summary>
        /// Minimum corner of the bounding box in world coordinates
        /// </summary>
        public Vector3<float> Min => Parent.Transform.Position + MinLocal;

        /// <summary>
        /// Maximum corner of the bounding box in world coordinates
        /// </summary>
        public Vector3<float> Max => Parent.Transform.Position + MaxLocal;

        /// <summary>
        /// Center of the bounding box in world coordinates
        /// </summary>
        public Vector3<float> Center => Parent.Transform.Position + CenterLocal;

        public BoundingBox(GameObject parent, Vector3<float> size) : base(parent)
        {
            MinLocal = -size / 2f;
            MaxLocal = size / 2f;
        }
    }
}
