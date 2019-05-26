using System;
using VulpineLib.Util;
using OpenTK.Graphics.OpenGL4;

using StarWander.GFX;

namespace StarWander.Effects
{
    public class TestTriangleEffect : Effect
    {
        private ShaderProgram? TriangleShaderProgram;
        private VertexAttributes? TriangleVertexAttributes;
        private Buffer<Vector3f>? TriangleVertexBuffer;
        private Texture2D? TriangleTexture;
        private int TriangleUniformTextureSampler;

        public TestTriangleEffect() : base("TestTriangleEffect")
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
            State.CheckError();

            TriangleShaderProgram = ShaderProgram.Create("TriangleShaderProgram");
            State.CheckError();

            using (var vert = Content.LoadShader("TriangleVertex.vert", GFX.ShaderType.Vertex))
            {
                using (var frag = Content.LoadShader("TriangleVertex.frag", GFX.ShaderType.Fragment))
                {
                    TriangleShaderProgram.AttachShaders(vert, frag);
                    TriangleShaderProgram.Link();
                    TriangleShaderProgram.DetachShaders(vert, frag);
                }
            }
            State.CheckError();

            TriangleUniformTextureSampler = TriangleShaderProgram.GetUniformLocation("textureSampler");
            State.CheckError();

            TriangleVertexBuffer = Buffer<Vector3f>.Create(
                    "TriangleVertexBuffer", 3, BufferStorageFlags.MapReadBit | BufferStorageFlags.MapWriteBit
                );
            State.CheckError();
            unsafe
            {
                var ptr = (Vector3f*)TriangleVertexBuffer.Map(BufferAccess.WriteOnly);
                State.CheckError();
                ptr[0] = new Vector3f(-1f, 1f, 0f);
                ptr[1] = new Vector3f(1f, 1f, 0f);
                ptr[2] = new Vector3f(0f, -1f, 0f);
                TriangleVertexBuffer.Unmap();
                State.CheckError();
            }

            TriangleVertexAttributes = VertexAttributes.Create("TriangleVertexAttributes");
            TriangleVertexAttributes.SetVertexAttributeEnabled(0, true);
            TriangleVertexAttributes.SetVertexAttributeLayout(0, "Position", GFX.AttributeType.Vector3, false, 0, 0, TriangleVertexBuffer);
            State.CheckError();

            TriangleTexture = Content.LoadTexture2D("rainbow.png");
        }

        protected override void OnUpdate(double time, double delta)
        {
            throw new NotImplementedException();
        }

        protected override void OnDraw(double time, double delta)
        {
            if (TriangleShaderProgram == null)
                throw new NullReferenceException();
            if (TriangleVertexAttributes == null)
                throw new NullReferenceException();
            if (TriangleVertexBuffer == null)
                throw new NullReferenceException();
            if (TriangleTexture == null)
                throw new NullReferenceException();

            TriangleVertexAttributes.Bind();
            TriangleShaderProgram.Bind();
            TriangleTexture.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            VertexAttributes.Unbind();
            ShaderProgram.Unbind();
            Texture2D.Unbind();
            State.CheckError();
        }

        protected override void OnDispose()
        {
            TriangleShaderProgram?.Dispose();
            TriangleVertexAttributes?.Dispose();
            TriangleVertexBuffer?.Dispose();
        }
    }
}
