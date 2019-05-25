using System;

namespace StarWander.GFX
{
    /// <summary>
    /// Exception thrown when something goes wrong during shader compilation
    /// </summary>
    internal class ShaderCompileException : Exception
    {
        /// <summary>
        /// The ShaderProgram the problem occured in
        /// </summary>
        public Shader Shader { get; private set; }

        public ShaderCompileException(Shader shader, string message) : base($"Error occured while compiling {shader}: " + message)
        {
            Shader = shader;
        }
    }
}
