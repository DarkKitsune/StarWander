namespace StarWander.Effects
{
    /*
    public class SpriteEffect : Effect
    {
        public struct Instance
        {
            public Matrix4 Matrix;
            public Sprite Sprite;
            public BlendType BlendType;

            public Instance(Matrix4 matrix, Sprite sprite, BlendType blendType)
            {
                Matrix = matrix;
                Sprite = sprite;
                BlendType = blendType;
            }
        }

        ShaderProgram? ShaderProgram;
        VertexAttributes? VertexAttributes;
        Instance[]? Instances;

        public SpriteEffect() : base("SpriteEffect")
        {

        }

        public override void SetParameter(string key, object value)
        {
            throw new NotImplementedException();
        }

        public override object GetParameter(string key)
        {
            throw new NotImplementedException();
        }

        protected override void OnInit(double time)
        {
            if (CameraBindingPoint == null)
                throw new NullReferenceException();

            ShaderProgram = ShaderProgram.Create("Sprite Shader Program");
            State.CheckError();

            using (var vert = Content.LoadShader(Path.Combine("shaders", "sprites.vert"), GFX.ShaderType.Vertex))
            {
                using (var frag = Content.LoadShader(Path.Combine("shaders", "sprites.frag"), GFX.ShaderType.Fragment))
                {
                    ShaderProgram.AttachShaders(vert, frag);
                    ShaderProgram.Link();
                    ShaderProgram.DetachShaders(vert, frag);
                }
            }
            State.CheckError();

            ShaderProgram.SetUniformBlockBindingPoint("Block_Camera", CameraBindingPoint);
            State.CheckError();

            VertexAttributes = VertexAttributes.Create("Tiles Vertex Attributes");
        }

        protected override void OnUpdate(double time, double delta)
        {
            throw new NotImplementedException();
        }

        protected override void OnDraw(double time, double delta)
        {
            if (ShaderProgram == null)
                throw new NullReferenceException();
            if (VertexAttributes == null)
                throw new NullReferenceException();
            if (Instances == null)
                throw new NullReferenceException();

            SpriteSet? curSpriteSet = null;
            BlendType.AlphaPremultiplied.Bind();
            ShaderProgram.Bind();
            VertexAttributes.Bind();
            foreach (var inst in Instances)
            {
                if (inst.Sprite == null)
                    continue;
                if (inst.Sprite.SpriteSet != curSpriteSet)
                {
                    curSpriteSet = inst.Sprite.SpriteSet;
                    if (curSpriteSet.Texture == null)
                        throw new NullReferenceException($"SpriteSet {curSpriteSet.Name} has a null texture");
                    curSpriteSet.Texture.Bind();
                }
                ShaderProgram.Uniform("uniform_spriteSourceTopLeft", inst.Sprite.TopLeftCoord);
                ShaderProgram.Uniform("uniform_spriteSourceSize", inst.Sprite.SizeCoord);
                ShaderProgram.Uniform("uniform_spritePosition", inst.Position);
                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            }
            ShaderProgram.Unbind();
            Texture2D.Unbind();
            VertexAttributes.Unbind();
            BlendType.Unbind();
            State.CheckError();
        }

        protected override void OnDispose()
        {
            ShaderProgram?.Dispose();
            VertexAttributes?.Dispose();
        }

        public void SetInstances(Instance[] instances)
        {
            Instances = instances;
        }
    }*/
}
