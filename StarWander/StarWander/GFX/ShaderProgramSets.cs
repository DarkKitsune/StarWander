using System.IO;
using System.Collections.Generic;

namespace StarWander.GFX
{
    public static class ShaderProgramSets
    {
        public static ShaderProgramSet Meshes { get; private set; }

        public static void Init()
        {
            Meshes = new ShaderProgramSet("Meshes", new Dictionary<string, ShaderProgram>()
            {
                { "BasicLit", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "basic", "BasicLit.vert"),
                        Path.Combine("shaders", "mesh", "basic", "BasicLit.frag")
                    )}
            });
        }
    }
}
