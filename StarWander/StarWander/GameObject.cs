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
            BoundingBox = new BoundingBox(this, Vector3<float>.Zero);

            OnStart();
        }

        /// <summary>
        /// Called when the object is created and set up
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Delete the object
        /// </summary>
        public void Delete()
        {
            OnEnd();
        }

        /// <summary>
        /// Called when the object is deleted
        /// </summary>
        protected virtual void OnEnd()
        {
        }

        /// <summary>
        /// Update the object
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        public void Update(double time, double delta)
        {
            OnUpdate(time, delta);
        }

        /// <summary>
        /// Called when updating the object
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        protected virtual void OnUpdate(double time, double delta)
        {
        }

        public override string ToString()
        {
            return $"[{nameof(GameObject)}]";
        }
    }
}
