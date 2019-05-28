using System;
using VulpineLib.Util;

namespace StarWander.Components
{
    public class Transform : Component
    {
        private Matrix4<float> _matrix = Matrix4<float>.Identity;
        private Quaternion<float> _rotation = Quaternion<float>.Identity;
        private Vector3<float> _position;
        private bool NeedsUpdate;

        public Quaternion<float> Rotation
        {
            get => _rotation;
            private set
            {
                _rotation = value;
                FlagForMatrixUpdate();
            }
        }

        public Matrix4<float> RotationMatrix => Rotation.Matrix;

        public Vector3<float> ForwardLocal => Vector3<float>.UnitX;
        public Vector3<float> RightLocal => -Vector3<float>.UnitY;
        public Vector3<float> UpLocal => Vector3<float>.UnitZ;

        /// <summary>
        /// Position of the object
        /// </summary>
        public Vector3<float> Position
        {
            get => _position;
            set
            {
                _position = value;
                FlagForMatrixUpdate();
            }
        }

        /// <summary>
        /// The forward normal of the object
        /// </summary>
        public Vector3<float> Forward => ForwardLocal * RotationMatrix;

        /// <summary>
        /// The up normal of the object
        /// </summary>
        public Vector3<float> Up => UpLocal * RotationMatrix;

        /// <summary>
        /// The up normal of the object
        /// </summary>
        public Vector3<float> Right => RightLocal * RotationMatrix;

        /// <summary>
        /// The backward normal of the object
        /// </summary>
        public Vector3<float> Backward => -Forward;

        /// <summary>
        /// The down normal of the object
        /// </summary>
        public Vector3<float> Down => -Up;

        /// <summary>
        /// The left normal of the object
        /// </summary>
        public Vector3<float> Left => -Right;

        /// <summary>
        /// The matrix representing this transformation
        /// </summary>
        public Matrix4<float> Matrix
        {
            get
            {
                DoMatrixUpdate();
                return _matrix;
            }
            private set
            {
                NeedsUpdate = false;
                _matrix = value;
            }
        }

        /// <summary>
        /// Flag the matrix for updating the next time it's accessed
        /// </summary>
        private void FlagForMatrixUpdate()
        {
            NeedsUpdate = true;
        }

        /// <summary>
        /// Check if the matrix needs updating and possibly update it
        /// </summary>
        private void DoMatrixUpdate()
        {
            if (!NeedsUpdate)
                return;

            Matrix = BuildMatrix();
        }

        /// <summary>
        /// Build a matrix representing the transformation
        /// </summary>
        private Matrix4<float> BuildMatrix()
        {
            return RotationMatrix.Transform(Matrix4<float>.CreateTranslation(Position));
        }

        public Transform(GameObject parent, Vector3<float> position) : base(parent)
        {
            Position = position;
        }

        /// <summary>
        /// Rotate the transform on the world X axis
        /// </summary>
        /// <param name="radians"></param>
        public void RotateX(double radians)
        {
            Rotation = Rotation.Rotate(Quaternion<float>.CreateRotationX(radians));
        }

        /// <summary>
        /// Rotate the transform on the world Y axis
        /// </summary>
        /// <param name="radians"></param>
        public void RotateY(double radians)
        {
            Rotation = Rotation.Rotate(Quaternion<float>.CreateRotationY(radians));
        }

        /// <summary>
        /// Rotate the transform on the world Z axis
        /// </summary>
        /// <param name="radians"></param>
        public void RotateZ(double radians)
        {
            Rotation = Rotation.Rotate(Quaternion<float>.CreateRotationZ(radians));
        }

        /// <summary>
        /// Rotate the transform on an arbitrary world axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="radians"></param>
        public void Rotate(Vector3<float> axis, double radians)
        {
            Rotation = Rotation.Rotate(Quaternion<float>.CreateRotation(axis, radians));
        }

        /// <summary>
        /// Reset the rotation of the transform
        /// </summary>
        public void ResetRotation()
        {
            Rotation = Quaternion<float>.Identity;
        }
    }
}
