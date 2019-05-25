using System;

namespace StarWander.GFX
{
    /// <summary>
    /// All the supported types of shaders
    /// </summary>
    public enum ShaderType
    {
        None,
        Vertex,
        Fragment
    }

    /// <summary>
    /// Extension methods for the ShaderType enum
    /// </summary>
    public static class ShaderTypeExtensions
    {
        /// <summary>
        /// Convert to the corresponding GL shader type
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static OpenTK.Graphics.OpenGL4.ShaderType ToGLShaderType(this ShaderType type)
        {
            switch (type)
            {
                case ShaderType.Vertex:
                    return OpenTK.Graphics.OpenGL4.ShaderType.VertexShader;
                case ShaderType.Fragment:
                    return OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader;
                default:
                    throw new NotImplementedException($"No GL ShaderType implemented for ShaderType {type}");
            }
        }
    }
}
