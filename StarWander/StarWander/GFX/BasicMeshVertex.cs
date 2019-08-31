using System;
using System.Collections.Generic;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public struct BasicMeshVertex : IVertex<Vector3<float>>
    {
        /// <summary>
        /// The vertex components
        /// </summary>
        public object[] Components => new object[] { Position, Normal, TexCoord };

        Vector3<float> IVertex<Vector3<float>>.V0
        {
            get => Position;
            set
            {
                Position = value;
            }
        }

        /// <summary>
        /// The local position of the vertex
        /// </summary>
        public Vector3<float> Position { get; set; }

        /// <summary>
        /// The normal of the vertex
        /// </summary>
        public Vector3<float> Normal { get; set; }

        /// <summary>
        /// Texture coordinates
        /// </summary>
        public Vector2<float> TexCoord { get; set; }

        public static BasicMeshVertex operator +(BasicMeshVertex a, BasicMeshVertex b)
        {
            a.Position += b.Position;
            a.Normal += b.Normal;
            a.TexCoord += b.TexCoord;
            return a;
        }

        public static BasicMeshVertex operator -(BasicMeshVertex a, BasicMeshVertex b)
        {
            a.Position -= b.Position;
            a.Normal -= b.Normal;
            a.TexCoord -= b.TexCoord;
            return a;
        }

        public static BasicMeshVertex operator *(BasicMeshVertex a, BasicMeshVertex b)
        {
            a.Position *= b.Position;
            a.Normal *= b.Normal;
            a.TexCoord *= b.TexCoord;
            return a;
        }

        public static BasicMeshVertex operator /(BasicMeshVertex a, BasicMeshVertex b)
        {
            a.Position /= b.Position;
            a.Normal /= b.Normal;
            a.TexCoord /= b.TexCoord;
            return a;
        }

        public static BasicMeshVertex operator +(BasicMeshVertex a, float b)
        {
            a.Position += b;
            a.Normal += b;
            a.TexCoord += b;
            return a;
        }

        public static BasicMeshVertex operator -(BasicMeshVertex a, float b)
        {
            a.Position -= b;
            a.Normal -= b;
            a.TexCoord -= b;
            return a;
        }

        public static BasicMeshVertex operator *(BasicMeshVertex a, float b)
        {
            a.Position *= b;
            a.Normal *= b;
            a.TexCoord *= b;
            return a;
        }

        public static BasicMeshVertex operator /(BasicMeshVertex a, float b)
        {
            a.Position /= b;
            a.Normal /= b;
            a.TexCoord /= b;
            return a;
        }
    }
}
