using System;
using VulpineLib.Util;
using VulpineLib.Animation;
using OpenTK.Graphics.OpenGL4;
using StarWander.GFX;
using StarWander.GameObjects.Actors.Ships;

namespace StarWander.Scenes
{
    class GameScene : Scene
    {
        private PlayerShip? PlayerShip;

        public GameScene() : base(nameof(GameScene))
        {
        }

        protected override void OnStart()
        {
            PlayerShip = new PlayerShip(Vector3<float>.Zero);
        }

        protected override void OnEnd()
        {
        }

        protected override void OnUpdate(double time, double deltaTime)
        {
        }

        protected override void OnDraw(double time, double deltaTime)
        {
            // Push a new first person camera
            if (PlayerShip is null)
                throw new NullReferenceException();
            Camera.Push();
            PlayerShip.SetCamera();

            // TODO: Draw demo stuff in first person here, moved forward in the X direction so it's not inside the camera

            Camera.Pop();
            // No longer in first person
        }
    }
}
