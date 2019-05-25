using System;
using VulpineLib.Util;

namespace StarWander.Components
{
    public class Transform : Component
    {
        private Matrix4<float> _matrix = Matrix4<float>.Identity;
        private Vector3<float> _position;
        private bool NeedsUpdate;

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
            return Matrix4<float>.CreateTranslation(Position);
        }

        public Transform(GameObject parent, Vector3<float> position) : base(parent)
        {
            Position = position;
        }
    }
}
