using System;
using VulpineLib.Util;

namespace StarWander.Components
{
    public class PositionalRandom : Component
    {
        /// <summary>
        /// The amount on each axis between positions with unique seeds (every X units a new seed is produced)
        /// </summary>
        const double PositionSeedTruncation = 0.001;

        /// <summary>
        /// The unique seed for this component
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// Random number generator seeded with the positional seed
        /// </summary>
        public Random Random { get; }

        public PositionalRandom(GameObject parent) : base(parent)
        {
            Seed = (Parent.Transform.PositionInRegion.To<double>() / PositionSeedTruncation).To<int>().GetHashCode();
            Random = new Random(Seed);
        }
    }
}
