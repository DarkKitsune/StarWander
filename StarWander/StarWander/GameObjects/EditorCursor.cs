using System;
using VulpineLib.Util;
using StarWander.GFX;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GameObjects
{
    public class EditorCursor : GameObject, IDisposable
    {
        public static ShaderProgram ShaderProgram => ShaderProgramSets.Meshes["SpriteTexturedLit"];
        public static Sprite? CursorSprite => SpriteSets.Editor["Cursor"];

        private Vector3<long> _point1;
        private Vector3<long> _point2;

        public bool Disposed { get; private set; }
        private MeshRenderer<BasicMeshVertex> MeshRenderer { get; }
        private GPUMesh<BasicMeshVertex> Mesh { get; }

        public Vector3<long> Point1
        {
            set
            {
                _point1 = value;
                SetTransform();
            }
            get
            {
                return _point1;
            }
        }


        public Vector3<long> Point2
        {
            set
            {
                _point2 = value;
                SetTransform();
            }
            get
            {
                return _point2;
            }
        }

        public EditorCursor(Region region, Vector3<decimal> position)
            : base(region, position)
        {
            MeshRenderer = new MeshRenderer<BasicMeshVertex>($"{nameof(EditorCursor)}.{nameof(MeshRenderer)}");
            var vertices = new BasicMeshVertex[]
            {
                // x-
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 1f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 1f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 0f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 1f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 0f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 0f),
                    Normal = new Vector3<float>(-1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                // x+
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 1f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 1f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 0f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 1f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 0f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 0f),
                    Normal = new Vector3<float>(1f, 0f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                // y-
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 0f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 0f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
                // y+
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 1f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 0f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(0f, 1f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(1f, 1f)
                },
                new BasicMeshVertex
                {
                    Position = new Vector3<float>(1f, 1f, 0f),
                    Normal = new Vector3<float>(0f, -1f, 0f),
                    TexCoord = new Vector2<float>(0f, 1f)
                },
            };
            var mesh = new Mesh<BasicMeshVertex>($"{nameof(World)} program-side mesh", vertices, MeshPrimitiveType.Triangles, false);
            Mesh = new GPUMesh<BasicMeshVertex>(mesh, ShaderProgram);
        }

        ~EditorCursor()
        {
            Dispose();
        }

        private void SetTransform()
        {
            Transform.Position = (Point1.To<decimal>() + Point2.To<decimal>()) / 2.0m;
            var diff = (Point2 - Point1);
            diff = new Vector3<long>(Math.Abs(diff.X), Math.Abs(diff.Y), Math.Abs(diff.Z));
            Transform.Scale = diff.To<float>() + 1.01f;
        }

        protected override void OnStart()
        {
            Input.GamePad.ButtonPress += GamePad_ButtonPress;
        }

        protected override void OnEnd()
        {
            Input.GamePad.ButtonPress -= GamePad_ButtonPress;
        }

        private bool GamePad_ButtonPress(string key)
        {
            return false;
        }

        private Vector2<float> _LastMove = Vector2<float>.Zero;
        protected override void OnUpdate(double time, double delta)
        {
            var rStick = Input.GamePad.RightStick;
            var move = rStick;
            if (move.LengthSquared > 0.001f && (_LastMove.LengthSquared <= 0.001f || move.Dot(_LastMove) < 0.75f))
            {
                Point1 += new Vector3<double>(move.Normalized * 1.5).To<long>();
                Point2 = Point1;
            }
            _LastMove = move;
        }

        protected override void OnDraw(double time, double delta)
        {
            if (CursorSprite is null)
                return;
            if (MeshRenderer is null)
                throw new NullReferenceException("MeshRenderer is null");
            if (CursorSprite.SpriteSet.Texture is null)
                throw new NullReferenceException("CursorSprite's spriteset has a null texture");

            // Disable depth testing and writing
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            // Render
            MeshRenderer.Begin();
            ShaderProgram.SetTextureSource(CursorSprite.TopLeftCoord, CursorSprite.SizeCoord);
            CursorSprite.SpriteSet.Texture.BindDiffuse();
            var trans = Transform.Matrix;
            MeshRenderer.DrawMesh(Mesh, ref trans, Color.White, BlendType.AlphaPremultiplied);
            MeshRenderer.End();
            // Enable depth settings
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            MeshRenderer.Dispose();
            Mesh.Dispose();
        }
    }
}
