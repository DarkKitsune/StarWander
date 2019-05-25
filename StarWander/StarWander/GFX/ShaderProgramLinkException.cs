using System;

namespace StarWander.GFX
{
    /// <summary>
    /// Exception thrown when something goes wrong during shader program linking
    /// </summary>
    internal class ShaderProgramLinkException : Exception
    {
        /// <summary>
        /// The ShaderProgram the problem occured in
        /// </summary>
        public ShaderProgram ShaderProgram { get; private set; }

        public ShaderProgramLinkException(ShaderProgram program, string message) : base($"Error occured while linking {program}: " + message)
        {
            ShaderProgram = program;
        }
    }
}
