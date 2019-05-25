using System;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    /// <summary>
    /// The supported types of vertex attributes
    /// </summary>
    public enum AttributeType
    {
        None,
        /// <summary>
        /// 1 float
        /// </summary>
        Float,
        /// <summary>
        /// 2 floats
        /// </summary>
        Vector2,
        /// <summary>
        /// 3 floats
        /// </summary>
        Vector3,
        /// <summary>
        /// 4 floats
        /// </summary>
        Vector4,
        /// <summary>
        /// 1 integer
        /// </summary>
        Int,
        /// <summary>
        /// 1 double
        /// </summary>
        Double,
        /// <summary>
        /// 2 doubles
        /// </summary>
        Vector2D,
        /// <summary>
        /// 3 doubles
        /// </summary>
        Vector3D,
        /// <summary>
        /// 4 doubles
        /// </summary>
        Vector4D
    }

    /// <summary>
    /// Provides extensions for the AttributeType enum
    /// </summary>
    public static class AttributeTypeExtensions
    {
        /// <summary>
        /// Convert to a VertexAttribPointerType and size for GL.VertexAttribPointer
        /// </summary>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="size">The size in value count</param>
        /// <exception cref="NotImplementedException"></exception>
        public static VertexAttribPointerType ToVertexAttribPointerType(this AttributeType attributeType, out int size)
        {
            switch (attributeType)
            {
                case AttributeType.Float:
                    size = 1;
                    return VertexAttribPointerType.Float;
                case AttributeType.Vector2:
                    size = 2;
                    return VertexAttribPointerType.Float;
                case AttributeType.Vector3:
                    size = 3;
                    return VertexAttribPointerType.Float;
                case AttributeType.Vector4:
                    size = 4;
                    return VertexAttribPointerType.Float;
                case AttributeType.Int:
                    size = 1;
                    return VertexAttribPointerType.Int;
                case AttributeType.Double:
                    size = 1;
                    return VertexAttribPointerType.Double;
                case AttributeType.Vector2D:
                    size = 2;
                    return VertexAttribPointerType.Double;
                case AttributeType.Vector3D:
                    size = 3;
                    return VertexAttribPointerType.Double;
                case AttributeType.Vector4D:
                    size = 4;
                    return VertexAttribPointerType.Double;
                default:
                    throw new NotImplementedException($"No VertexAttribType implemented to match {attributeType}");
            }
        }
    }
}
