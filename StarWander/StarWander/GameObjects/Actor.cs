using System;
using VulpineLib.Util;
using StarWander.Components;

namespace StarWander.GameObjects
{
    public class Actor : GameObject
    {
        /// <summary>
        /// The actor's health pool
        /// </summary>
        public HealthPool HealthPool { get; }

        /// <summary>
        /// Object the actor was last damaged by
        /// </summary>
        public GameObject? LastDamagedBy { get; private set; }

        public Actor(Vector3<float> position, double health) : base(position)
        {
            HealthPool = new HealthPool(this, health);
            HealthPool.Depleted += Kill;
        }

        private void Kill(HealthPool healthPool)
        {
            OnKill();
        }

        /// <summary>
        /// Called when the actor is killed
        /// </summary>
        protected virtual void OnKill()
        {
        }

        public void Damage(GameObject? attacker, double amount)
        {
            HealthPool.Damage(amount);
            LastDamagedBy = attacker;
            OnDamage(attacker, amount);
        }

        /// <summary>
        /// Called when the actor is damaged
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="amount"></param>
        protected virtual void OnDamage(GameObject? attacker, double amount)
        {
        }

        public override string ToString()
        {
            return $"[{nameof(Actor)}]";
        }
    }
}
