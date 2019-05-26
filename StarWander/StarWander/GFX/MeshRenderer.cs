using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using VulpineLib.Util;

namespace StarWander.GFX
{
    internal class MeshRenderer<T> : Renderer
        where T : struct, IVertex
    {
        /*
        public TextureMinFilter MinFilter
        {
            get
            {
                Assert();

                return DiffuseSampler.MinFilter;
            }
            set
            {
                Assert();

                DiffuseSampler.MinFilter = value;
            }
        }

        public TextureMagFilter MagFilter
        {
            get
            {
                Assert();

                return DiffuseSampler.MagFilter;
            }
            set
            {
                Assert();

                DiffuseSampler.MagFilter = value;
            }
        }*/

        /// <summary>
        /// Construct a MeshRenderer
        /// </summary>
        /// <param name="name"></param>
        public MeshRenderer(string name) : base(name)
        {
            //DiffuseSampler = Sampler.Create($"{Name} MeshRenderer Diffuse Sampler");
        }

        ~MeshRenderer()
        {
            Dispose();
        }

        //private Sampler DiffuseSampler;
        private GPUMesh<T>? LastMesh;

        /// <summary>
        /// Begin drawing meshes
        /// </summary>
        protected override void OnBegin()
        {
            //DiffuseSampler.BindDiffuse();
            // Reset previous Sprite/SpriteSet cache
            LastMesh = null;
            State.CheckError();
        }

        /// <summary>
        /// Draw a mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="transform"></param>
        /// <param name="color"></param>
        /// <param name="blendType"></param>
        public void DrawMesh(GPUMesh<T> mesh, ref Matrix4<float> transform, VulpineLib.Util.Color color, BlendType blendType)
        {
            Assert();

            if (!Drawing)
                throw new InvalidOperationException("Not drawing; Begin() must be called first");
            if (mesh != LastMesh!)
            {
                mesh.Bind();
                LastMesh = mesh;
            }
            // Bind the BlendType
            blendType.Bind();
            // Set the model color
            mesh.ShaderProgram.SetModelColor(color);
            // Set the model matrix
            mesh.ShaderProgram.SetModelMatrix(ref transform);
            // Draw
            mesh.Draw();
            State.CheckError();
        }

        /// <summary>
        /// Finish drawing meshes
        /// </summary>
        protected override void OnEnd()
        {
            GPUMesh<T>.Unbind();
            Texture2D.UnbindDiffuse();
            Sampler.UnbindDiffuse();
            BlendType.Unbind();
            State.CheckError();
        }

        protected override void OnDispose()
        {
            //DiffuseSampler.Dispose();
        }
    }
}
