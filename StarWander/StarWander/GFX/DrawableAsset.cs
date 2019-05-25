using System;
using System.Collections.Generic;
using System.Text;

namespace StarWander.GFX
{
    public class DrawableAsset : Asset
    {
        /// <summary>
        /// Shader to use when drawing
        /// </summary>
        public ShaderProgram ShaderProgram { get; set; }

        private DrawableAsset() : base("")
        {
            ShaderProgram = ShaderProgram.None;
        }

        /// <summary>
        /// Create a DrawableAsset
        /// </summary>
        /// <param name="name"></param>
        public DrawableAsset(string name, ShaderProgram shaderProgram) : base(name)
        {
            ShaderProgram = shaderProgram;
        }

        protected override void OnDispose()
        {
            ShaderProgram.Dispose();
        }
    }
}
