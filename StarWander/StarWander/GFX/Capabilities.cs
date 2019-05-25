
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    public class Capabilities
    {
        public int MaxVaryingComponents { get; private set; }
        public int MaxVertexAttributes { get; private set; }
        public Capabilities()
        {
            MaxVaryingComponents = GL.GetInteger(GetPName.MaxVaryingComponents);
            MaxVertexAttributes = GL.GetInteger(GetPName.MaxVertexAttribs);
        }
    }
}
