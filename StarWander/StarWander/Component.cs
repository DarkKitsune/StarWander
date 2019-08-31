using System;

namespace StarWander
{
    public class Component
    {
        public GameObject Parent { get; }

        public Component(GameObject parent)
        {
            Parent = parent;
            Parent.RegisterComponent(this);
        }

        /// <summary>
        /// Call when starting the component
        /// Don't use unless you know what you're doing
        /// </summary>
        public void Start()
        {
            OnStart();
        }

        /// <summary>
        /// Called when the component is started
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Call when ending the component
        /// Don't use unless you know what you're doing
        /// </summary>
        public void End()
        {
            OnEnd();
        }

        /// <summary>
        /// Called when the component is ended
        /// </summary>
        protected virtual void OnEnd()
        {
        }
    }
}
