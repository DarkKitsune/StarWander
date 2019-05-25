using System;
using StarWander.GFX;
using VulpineLib.Util;

namespace StarWander.GameObjects.Actors.Ships
{
    public class PlayerShip : Ship
    {
        private const double StartingHealth = 100.0;

        /// <summary>
        /// Set the top of the camera stack to a first-person camera at this ship
        /// </summary>
        public void SetCamera()
        {
            Camera.Set(
                    Matrix4<float>.CreateLookAt(Transform.Position, Vector3<float>.UnitX, Vector3<float>.UnitZ),
                    Matrix4<float>.CreatePerspective(Math.PI / 2.0, 1.0, 0.01, 1000.0)
                );
        }

        public PlayerShip(Vector3<float> position) : base(position, StartingHealth)
        {
        }
    }
}
