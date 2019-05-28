using System;
using StarWander.GFX;
using VulpineLib.Util;
using StarWander.Input;
using OpenTK.Input;

namespace StarWander.GameObjects.Actors.Ships
{
    public class PlayerShip : Ship
    {
        private const double StartingHealth = 100.0;

        public double RollSpeed => Math.PI;
        public double YawSpeed => Math.PI;
        public double PitchSpeed => Math.PI;
        public double MoveSpeed => 4.0;

        /// <summary>
        /// Set the top of the camera stack to a first-person camera at this ship
        /// </summary>
        public void SetCamera(double aspectRatio)
        {
            Camera.Set(
                    Matrix4<float>.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up),
                    Matrix4<float>.CreatePerspective(Math.PI / 3.0, aspectRatio, 0.001, 1000.0)
                );
        }

        public PlayerShip(Vector3<float> position) : base(position, StartingHealth)
        {
        }

        protected override void OnUpdate(double time, double delta)
        {
            var rollInput = (Input.GamePad.IsButtonPressed("RightShoulder") ? 1.0 : 0.0) - (Input.GamePad.IsButtonPressed("LeftShoulder") ? 1.0 : 0.0);
            var yawPitchInput = Input.GamePad.RightStick;
            var forwardInput = Input.GamePad.RightTrigger - Input.GamePad.LeftTrigger;
            var rludInput = Input.GamePad.LeftStick;


            if (!rollInput.EqualsSafe(0f, 0.0001f))
            {
                Transform.Rotate(Transform.Forward, rollInput * RollSpeed * delta);
            }
            if (yawPitchInput.LengthSquared > 0.0001f)
            {
                Transform.Rotate(Transform.Up, yawPitchInput.X * -YawSpeed * delta);
                Transform.Rotate(Transform.Right, yawPitchInput.Y * PitchSpeed * delta);
            }
            if (rludInput.LengthSquared > 0.0001f)
            {
                Transform.Position += Transform.Up * rludInput.Y * (float)MoveSpeed * (float)delta
                    + Transform.Right * rludInput.X * (float)MoveSpeed * (float)delta;
            }
            if (!forwardInput.EqualsSafe(0f, 0.0001f))
            {
                Transform.Position += Transform.Forward * forwardInput * (float)MoveSpeed * (float)delta;
            }
            if (Input.GamePad.IsButtonPressed("Y"))
            {
                Transform.ResetRotation();
            }
            if (Input.GamePad.IsButtonPressed("X"))
            {
                Console.WriteLine(Transform.Forward);
            }
        }
    }
}
