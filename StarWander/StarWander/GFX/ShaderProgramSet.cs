using System;
using System.Collections.Generic;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class ShaderProgramSet : INameable
    {
        /// <summary>
        /// The ShaderProgramSet's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The collection of shader programs
        /// </summary>
        private Dictionary<string, ShaderProgram> ShaderPrograms = new Dictionary<string, ShaderProgram>();

        /// <summary>
        /// Get a collection of the sprites
        /// </summary>
        public Dictionary<string, ShaderProgram>.ValueCollection Collection => ShaderPrograms.Values;

        public ShaderProgram this[string name]
        {
            get
            {
                return GetShaderProgram(name);
            }
        }

        public ShaderProgramSet(string name)
        {
            Name = name;
            ShaderPrograms = new Dictionary<string, ShaderProgram>();
        }

        public ShaderProgramSet(string name, Dictionary<string, ShaderProgram> programs)
        {
            Name = name;
            ShaderPrograms = programs;
        }

        /// <summary>
        /// Set a ShaderProgram in the set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shaderProgram"></param>
        public void SetShaderProgram(string name, ShaderProgram shaderProgram)
        {
            ShaderPrograms[name] = shaderProgram;
        }

        /// <summary>
        /// Get a ShaderProgram in the set
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <exception cref="ArgumentException"></exception>
        public ShaderProgram GetShaderProgram(string name)
        {
            if (ShaderPrograms.TryGetValue(name, out var shaderProgram))
                return shaderProgram;
            throw new ArgumentException($"{nameof(ShaderProgram)} with name \"{name}\" doesn't exist in this {nameof(ShaderProgramSet)}");
        }
    }
}
