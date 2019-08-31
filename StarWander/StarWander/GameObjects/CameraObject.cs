using System;
using StarWander.GFX;
using VulpineLib.Util;
using StarWander.Components;

namespace StarWander.GameObjects
{
    public class CameraObject : GameObject
    {
        public static CameraObject Current;

        public float FOV = MathF.PI / 2.5f;
        public float Zoom = 1f;

        public CameraObject(Region region, Vector3<decimal> position) : base(region, position)
        {
            Current = this;
        }

        /// <summary>
        /// Set the top of the camera stack to this camera
        /// </summary>
        public void SetCamera(Region region, float aspectRatio, float near, float far)
        {
            var pos = Transform.GetPositionInRegion(region).To<float>();
            Camera.SetProjection(
                    pos, pos + Transform.Forward,
                    MathF.Atan(MathF.Tan(FOV) / Zoom), aspectRatio, Transform.Up,
                    near,
                    far
                );
        }

        public override string ToString()
        {
            return $"[{nameof(CameraObject)}]";
        }
    }
}
