using System;
using VulpineLib.Util;

namespace StarWander.GameObjects.CameraObjects
{
    public class MovableCamera : CameraObject
    {
        public float MoveSpeed = (float)Conversion.Convert(Conversion.UnitType.Meters, Conversion.UnitType.EngineUnits, 20m);
        public double RollSpeed = Math.PI;
        public double YawSpeed = Math.PI;
        public double PitchSpeed = Math.PI;

        public MovableCamera(Region region, Vector3<decimal> position) : base(region, position)
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
                Transform.PositionInRegion += (Transform.Up * rludInput.Y * MoveSpeed * (float)delta
                    + Transform.Right * rludInput.X * MoveSpeed * (float)delta).To<decimal>();
            }
            if (!forwardInput.EqualsSafe(0f, 0.0001f))
            {
                Console.WriteLine("A");
                Transform.PositionInRegion += (Transform.Forward * forwardInput * MoveSpeed * (float)delta).To<decimal>();
            }
        }
    }
}
