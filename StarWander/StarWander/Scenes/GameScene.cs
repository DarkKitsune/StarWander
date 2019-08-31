using System;
using VulpineLib.Util;
using StarWander.GFX;
using StarWander.GameObjects;
using StarWander.GameObjects.Actors.SpriteActors;

namespace StarWander.Scenes
{
    public class GameScene : Scene
    {
        private World? World;
        private CameraObject? Camera;
        private Player? Player;
        private EditorCursor? EditorCursor;

        public GameScene() : base(nameof(GameScene))
        {
        }

        protected override void OnStart()
        {
            var spawn = new Vector3<decimal>(0.5m, 0.5m, 0m);

            // Create world that is 1 kilometer on every axis
            World = new World(
                    Vector3<decimal>.Zero,
                    new Vector3<decimal>(
                            new Vector2<decimal>(100),
                            5
                        ),
                    new Vector3<decimal>(
                            150
                        )
                );

            // Generate world tiles
            World.Generate();
            // Build world mesh
            World.BuildMesh();

            // Add camera to the world
            Camera = new CameraObject(World, Vector3<decimal>.Zero);
            World.Add(Camera);
            // Rotate the camera and move it to aim at (0, 0, 0) from 10 meters away
            Camera.Transform.YawLeft(Math.PI * 0.5);
            Camera.Transform.PitchDown(Math.PI * 0.4);
            Camera.Transform.PositionInRegion = spawn
                - Camera.Transform.Forward.To<decimal>() * 6;
            Camera.FOV = MathF.PI / 2.4f;
            Camera.Zoom = 0.1f;

            // Place player object in world
            Player = new Player(
                    World,
                    spawn
                );
            World.Add(Player);

            // Place editor cursor if in edit mode
            if (SGameWindow.Main!.Mode == GameWindowMode.WorldEditor)
            {
                Log.Info("Created edit cursor", nameof(GameScene));
                EditorCursor = new EditorCursor(World, Vector3<decimal>.Zero);
                World.Add(EditorCursor);
            }
        }

        protected override void OnEnd()
        {
        }

        protected override void OnUpdate(double time, double deltaTime)
        {
            if (World is null)
                throw new NullReferenceException();

            World.Update(time, deltaTime);
        }

        protected override void OnDraw(double time, double deltaTime)
        {
            if (World is null)
                throw new NullReferenceException();

            World.Draw(time, deltaTime);
        }
    }
}
