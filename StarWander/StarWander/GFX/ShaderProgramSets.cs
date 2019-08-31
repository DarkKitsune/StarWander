using System.IO;
using System.Collections.Generic;

namespace StarWander.GFX
{
    public static class ShaderProgramSets
    {
        public static ShaderProgramSet Effects { get; private set; }
        public static ShaderProgramSet Meshes { get; private set; }
        public static ShaderProgramSet Planets { get; private set; }

        public static void Init()
        {
            Effects = new ShaderProgramSet("Effects", new Dictionary<string, ShaderProgram>()
            {
                { "City", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "effects", "city", "City.vert"),
                        Path.Combine("shaders", "effects", "city", "City.frag")
                    )}
            });

            Meshes = new ShaderProgramSet("Meshes", new Dictionary<string, ShaderProgram>()
            {
                { "BasicLit", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "basic", "BasicLit.vert"),
                        Path.Combine("shaders", "mesh", "basic", "BasicLit.frag")
                    )},
                { "BasicWireframe", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "basic", "BasicWireframe.vert"),
                        Path.Combine("shaders", "mesh", "basic", "BasicWireframe.frag")
                    )},
                { "TexturedLit", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "basic", "TexturedLit.vert"),
                        Path.Combine("shaders", "mesh", "basic", "TexturedLit.frag")
                    )},
                { "SpriteTexturedLit", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "basic", "SpriteTexturedLit.vert"),
                        Path.Combine("shaders", "mesh", "basic", "SpriteTexturedLit.frag")
                    )}
            });

            Planets = new ShaderProgramSet("Planets", new Dictionary<string, ShaderProgram>()
            {
                { "Atmospheric", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "planet", "Atmospheric.vert"),
                        Path.Combine("shaders", "mesh", "planet", "Atmospheric.frag")
                    )},
                { "Sun", Content.LoadShaderProgramVF(
                        Path.Combine("shaders", "mesh", "planet", "Sun.vert"),
                        Path.Combine("shaders", "mesh", "planet", "Sun.frag")
                    )}
            });
        }
    }
}
