using System;
using VulpineLib.Util;
using StarWander.Components;

namespace StarWander
{
    public class GameObject
    {
        /// <summary>
        /// The object's transformation
        /// </summary>
        public Transform Transform { get; }

        /// <summary>
        /// Bounding box of the object
        /// </summary>
        public BoundingBox BoundingBox { get; }

        public GameObject(Vector3<float> position)
        {
            Transform = new Transform(this, position);
        }

        public override string ToString()
        {
            return $"[{nameof(GameObject)}]";
        }
    }
}
