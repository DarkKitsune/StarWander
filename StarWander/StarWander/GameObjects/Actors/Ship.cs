using System;
using VulpineLib.Util;

namespace StarWander.GameObjects.Actors
{
    public class Ship : Actor
    {
        public Ship(Vector3<float> position, double health) : base(position, health)
        {
            
        }

        protected override void OnDamage(GameObject? attacker, double amount)
        {
            Log.Info($"Attacked by {attacker} for {amount} damage", nameof(Ship));
        }

        public override string ToString()
        {
            return $"[{nameof(Ship)}]";
        }
    }
}
