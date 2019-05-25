using System;

namespace StarWander.Components
{
    public class HealthPool : Component
    {
        private double _maxHealth;

        /// <summary>
        /// The current health in the pool
        /// </summary>
        public double Health { get; private set; }

        /// <summary>
        /// The maximum health in the pool
        /// </summary>
        public double MaxHealth
        {
            get => _maxHealth;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException($"Cannot set {nameof(MaxHealth)} to a value below 0");
                _maxHealth = value;
                if (Health > _maxHealth)
                    Health = _maxHealth;
            }
        }

        /// <summary>
        /// Invoked when the health is depleted
        /// </summary>
        public event Action<HealthPool> Depleted;

        /// <summary>
        /// Deal damage to the health pool (negative values heal)
        /// </summary>
        /// <param name="amount"></param>
        public void Damage(double amount)
        {
            if (Health == 0.0 && amount >= 0.0)
                return;
            Health = Math.Clamp(Health - amount, 0.0, MaxHealth);
            if (Health == 0.0)
            {
                foreach (var d in Depleted.GetInvocationList())
                    d.DynamicInvoke(this);
            }
        }

        public HealthPool(GameObject parent, double maxHealth) : base(parent)
        {
            Health = maxHealth;
            MaxHealth = maxHealth;
        }
    }
}
