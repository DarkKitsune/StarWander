using System;
using System.Collections.Generic;
using VulpineLib.Util;
using StarWander.Components;

namespace StarWander
{
    public class GameObject
    {
        public Region Region { get; }

        /// <summary>
        /// The object's transformation
        /// </summary>
        public Transform Transform { get; }

        /// <summary>
        /// Bounding box of the object
        /// </summary>
        public BoundingBox BoundingBox { get; }

        /// <summary>
        /// Registered components
        /// </summary>
        private List<Component> ComponentRegistry { get; } = new List<Component>();

        /// <summary>
        /// Components owned by this object
        /// </summary>
        public IEnumerable<Component> Components => ComponentRegistry;

        public GameObject(Region region, Vector3<decimal> position)
        {
            Region = region;
            Transform = new Transform(this, position);
            BoundingBox = new BoundingBox(this, Vector3<float>.Zero);
        }

        /// <summary>
        /// Start the object; required before doing anything with it
        /// </summary>
        public void Start()
        {
            foreach (var component in Components)
                component.Start();
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
            foreach (var component in Components)
                component.End();
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

        /// <summary>
        /// Draw the object
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        public void Draw(double time, double delta)
        {
            OnDraw(time, delta);
        }

        /// <summary>
        /// Called when drawing the object
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        protected virtual void OnDraw(double time, double delta)
        {
        }

        /// <summary>
        /// Register a component.
        /// Don't use unless you know what you're doing
        /// </summary>
        /// <param name="component"></param>
        public void RegisterComponent(Component component)
        {
            ComponentRegistry.Add(component);
        }

        /// <summary>
        /// Check if the object is within the bounds of a region
        /// (but not necessarily owned by the region)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsWithin(Region region)
        {
            var pos = Transform.Position;
            return pos.Between(region.DrawMin, region.DrawMax);
        }

        public override string ToString()
        {
            return $"[{nameof(GameObject)}]";
        }
    }
}
