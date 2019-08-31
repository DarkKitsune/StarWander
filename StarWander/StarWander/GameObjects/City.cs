using System;
using VulpineLib.Util;
using StarWander.GFX;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GameObjects
{
    public class City : GameObject
    {
        /// <summary>
        /// The City shader program
        /// </summary>
        internal static ShaderProgram ShaderProgram => ShaderProgramSets.Effects["City"];

        /// <summary>
        /// The object's mesh
        /// </summary>
        internal GPUMesh<BasicMeshVertex> Mesh { get; set; }

        internal MeshRenderer<BasicMeshVertex> MeshRenderer { get; }

        public City(Region region, Vector3<decimal> position, Vector2<float> size)
            : base(region, position)
        {
            Mesh = null!;
            BuildMesh(size);
            MeshRenderer = new MeshRenderer<BasicMeshVertex>($"{nameof(City)}.{nameof(MeshRenderer)}");
        }

        protected override void OnStart()
        {
        }

        protected override void OnEnd()
        {
            Mesh.Dispose();
        }

        protected override void OnDraw(double time, double delta)
        {
            base.OnDraw(time, delta);
            GL.Enable(EnableCap.DepthTest);
            MeshRenderer.Begin();
            ShaderProgram.Uniform("uniform_time", (float)time);
            var mat = Transform.Matrix;
            MeshRenderer.DrawMesh(Mesh, ref mat, Color.White, BlendType.Alpha);
            MeshRenderer.End();
        }

        // Mesh generation
        private void BuildMesh(Vector2<float> size)
        {
            // Create a box
            var box = new Mesh<BasicMeshVertex>(
                    "City",
                    new BasicMeshVertex[] {
                        new BasicMeshVertex {
                            Position = new Vector3<float>(size / -2f, 0f),
                            Normal = new Vector3<float>(0f, 0f, 1f),
                            TexCoord = new Vector2<float>(0f, 0f)
                        },
                        new BasicMeshVertex {
                            Position = new Vector3<float>(size / new Vector2<float>(2f, -2f), 0f),
                            Normal = new Vector3<float>(0f, 0f, 1f),
                            TexCoord = new Vector2<float>(1f, 0f)
                        },
                        new BasicMeshVertex {
                            Position = new Vector3<float>(size / 2f, 0f),
                            Normal = new Vector3<float>(0f, 0f, 1f),
                            TexCoord = new Vector2<float>(0f, 1f)
                        },
                        new BasicMeshVertex {
                            Position = new Vector3<float>(size / new Vector2<float>(-2f, 2f), 0f),
                            Normal = new Vector3<float>(0f, 0f, 1f),
                            TexCoord = new Vector2<float>(1f, 1f)
                        }
                    },
                    new int[] {
                        0, 1, 2,
                        2, 3, 0
                    },
                    MeshPrimitiveType.Triangles
                );

            // Make a GPUMesh
            Mesh = new GPUMesh<BasicMeshVertex>(box, ShaderProgram);
        }
    }
}
