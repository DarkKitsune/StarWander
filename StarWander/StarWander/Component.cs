using System;

namespace StarWander
{
    public class Component
    {
        public GameObject Parent { get; }

        public Component(GameObject parent)
        {
            Parent = parent;
        }
    }
}
