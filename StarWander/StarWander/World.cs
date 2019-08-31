using System;
using System.Collections.Generic;
using StarWander.GFX;
using VulpineLib.Util;
using OpenTK.Graphics.OpenGL4;

namespace StarWander
{
    public class World : Region, IDisposable
    {
        public static Vector3<long> ChunkSize => new Vector3<long>(20, 20, 4);
        public static ShaderProgram ShaderProgram => ShaderProgramSets.Meshes["TexturedLit"];

        public bool Disposed { get; private set; }
        private UnboundedGrid3D<Tile> Grid { get; }
        private GPUMesh<BasicMeshVertex>? Mesh { get; set; }
        private MeshRenderer<BasicMeshVertex> MeshRenderer { get; }
        private Sampler DiffuseSampler { get; }

        public World(Vector3<decimal> center, Vector3<decimal> size, Vector3<decimal> drawBoundingBox)
            : base(center, size, drawBoundingBox)
        {
            Grid = new UnboundedGrid3D<Tile>(ChunkSize.To<long>());
            MeshRenderer = new MeshRenderer<BasicMeshVertex>($"{nameof(World)}.{nameof(MeshRenderer)}");
            DiffuseSampler = Sampler.Create($"{nameof(World)} Mesh Diffuse Sampler");
        }

        ~World()
        {
            Dispose();
        }

        public Tile GetTile(Vector3<decimal> position)
        {
            var bpos = new Vector3<long>(
                    (long)Math.Floor(position.X),
                    (long)Math.Floor(position.Y),
                    (long)Math.Floor(position.Z)
                );
            return Grid[bpos];
        }

        public Tile GetTile(Vector3<long> position)
        {
            return Grid[position];
        }

        public void SetTile(Vector3<decimal> position, Tile value)
        {
            var bpos = new Vector3<long>(
                    (long)Math.Floor(position.X),
                    (long)Math.Floor(position.Y),
                    (long)Math.Floor(position.Z)
                );
            Grid[bpos] = value;
        }

        public void SetTile(Vector3<long> position, Tile value)
        {
            Grid[position] = value;
        }

        public void Generate()
        {
        }

        public void BuildMesh()
        {
            Mesh?.Dispose();

            // Create mesh vertices
            var vertices = BuildMeshVertices();
            if (vertices.Count == 0)
            {
                Mesh = null;
                return;
            }

            // Create program-side mesh
            var mesh = new Mesh<BasicMeshVertex>($"{nameof(World)} program-side mesh", vertices, MeshPrimitiveType.Triangles, false);

            // Create GPU-side mesh from program-side mesh
            Mesh = new GPUMesh<BasicMeshVertex>(mesh, ShaderProgram);
        }

        private List<BasicMeshVertex> BuildMeshVertices()
        {
            var vertices = new List<BasicMeshVertex>();
            var min = -Size.To<long>() / 2;
            var max = min + Size.To<long>();
            for (var z = min.Z; z < max.Z; z++)
            {
                for (var y = min.Y; y < max.Y; y++)
                {
                    Tile tile = Tile.None;
                    Tile xpTile = GetTile(new Vector3<long>(min.X, y, z));
                    for (var x = min.X; x < max.X; x++)
                    {
                        var xmTile = tile;
                        tile = xpTile;
                        xpTile = GetTile(new Vector3<long>(x + 1, y, z));
                        var tileData = tile.Data();
                        if (tileData.Invisible)
                            continue;
                        var tileSprite = (tileData.Sprite != null ? TileExtensions.SpriteSet[tileData.Sprite] : null);
                        if (tileSprite is null)
                            continue;
                        var drawXM = xmTile.Data().DoesNotBlockFaces;
                        var drawXP = xpTile.Data().DoesNotBlockFaces;
                        var drawYM = GetTile(new Vector3<long>(x, y - 1, z)).Data().DoesNotBlockFaces;
                        var drawYP = GetTile(new Vector3<long>(x, y + 1, z)).Data().DoesNotBlockFaces;
                        //var drawZM = GetTile(new Vector3<long>(x, y, z - 1)).Data().DoesNotBlockFaces;
                        var drawZP = GetTile(new Vector3<long>(x, y, z + 1)).Data().DoesNotBlockFaces;
                        var tc00 = tileSprite.TopLeftCoord + new Vector2<float>(0, 0);
                        var tc01 = tileSprite.TopLeftCoord + new Vector2<float>(0, tileSprite.SizeCoord.Y);
                        var tc10 = tileSprite.TopLeftCoord + new Vector2<float>(tileSprite.SizeCoord.X, 0);
                        var tc11 = tileSprite.TopLeftCoord + new Vector2<float>(tileSprite.SizeCoord.X, tileSprite.SizeCoord.Y);
                        var xm00 = new Vector3<float>(x, y, z + 1);
                        var xm01 = new Vector3<float>(x, y, z);
                        var xm10 = new Vector3<float>(x, y + 1, z + 1);
                        var xm11 = new Vector3<float>(x, y + 1, z);
                        var xmNorm = new Vector3<float>(-1, 0, 0);
                        var xp00 = new Vector3<float>(x + 1, y + 1, z + 1);
                        var xp01 = new Vector3<float>(x + 1, y + 1, z);
                        var xp10 = new Vector3<float>(x + 1, y, z + 1);
                        var xp11 = new Vector3<float>(x + 1, y, z);
                        var xpNorm = new Vector3<float>(1, 0, 0);
                        var ym00 = new Vector3<float>(x + 1, y, z + 1);
                        var ym01 = new Vector3<float>(x + 1, y, z);
                        var ym10 = new Vector3<float>(x, y, z + 1);
                        var ym11 = new Vector3<float>(x, y, z);
                        var ymNorm = new Vector3<float>(0, -1, 0);
                        var yp00 = new Vector3<float>(x, y + 1, z + 1);
                        var yp01 = new Vector3<float>(x, y + 1, z);
                        var yp10 = new Vector3<float>(x + 1, y + 1, z + 1);
                        var yp11 = new Vector3<float>(x + 1, y + 1, z);
                        var ypNorm = new Vector3<float>(0, 1, 0);
                        var zp00 = new Vector3<float>(x, y, z + 1);
                        var zp01 = new Vector3<float>(x, y + 1, z + 1);
                        var zp10 = new Vector3<float>(x + 1, y, z + 1);
                        var zp11 = new Vector3<float>(x + 1, y + 1, z + 1);
                        var zpNorm = new Vector3<float>(0, 0, 1);

                        // x-
                        if (drawXM)
                        {
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm10,
                                    Normal = xmNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm00,
                                    Normal = xmNorm,
                                    TexCoord = tc00,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm01,
                                    Normal = xmNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm10,
                                    Normal = xmNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm01,
                                    Normal = xmNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xm11,
                                    Normal = xmNorm,
                                    TexCoord = tc11,
                                }
                            );
                        }

                        // x+
                        if (drawXP)
                        {
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp10,
                                    Normal = xpNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp00,
                                    Normal = xpNorm,
                                    TexCoord = tc00,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp01,
                                    Normal = xpNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp10,
                                    Normal = xpNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp01,
                                    Normal = xpNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = xp11,
                                    Normal = xpNorm,
                                    TexCoord = tc11,
                                }
                            );
                        }

                        // y-
                        if (drawYM)
                        {
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym10,
                                    Normal = ymNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym00,
                                    Normal = ymNorm,
                                    TexCoord = tc00,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym01,
                                    Normal = ymNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym10,
                                    Normal = ymNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym01,
                                    Normal = ymNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = ym11,
                                    Normal = ymNorm,
                                    TexCoord = tc11,
                                }
                            );
                        }

                        // y+
                        if (drawYP)
                        {
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp10,
                                    Normal = ypNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp00,
                                    Normal = ypNorm,
                                    TexCoord = tc00,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp01,
                                    Normal = ypNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp10,
                                    Normal = ypNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp01,
                                    Normal = ypNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = yp11,
                                    Normal = ypNorm,
                                    TexCoord = tc11,
                                }
                            );
                        }

                        // z+
                        if (drawZP)
                        {
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp10,
                                    Normal = zpNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp00,
                                    Normal = zpNorm,
                                    TexCoord = tc00,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp01,
                                    Normal = zpNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp10,
                                    Normal = zpNorm,
                                    TexCoord = tc10,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp01,
                                    Normal = zpNorm,
                                    TexCoord = tc01,
                                }
                            );
                            vertices.Add(
                                new BasicMeshVertex
                                {
                                    Position = zp11,
                                    Normal = zpNorm,
                                    TexCoord = tc11,
                                }
                            );
                        }
                    }
                }
            }
            return vertices;
        }

        protected override void OnDraw(double time, double delta)
        {
            if (!(Mesh is null))
            {
                if (TileExtensions.SpriteSet.Texture is null)
                    throw new NullReferenceException("SpriteSet texture is null");
                // Enable depth testing
                GL.Enable(EnableCap.DepthTest);
                // Render
                MeshRenderer.Begin();
                TileExtensions.SpriteSet.Texture.BindDiffuse();
                DiffuseSampler.BindDiffuse();
                var trans = Matrix4<float>.Identity;
                MeshRenderer.DrawMesh(Mesh, ref trans, Color.White, BlendType.Alpha);
                MeshRenderer.End();
            }
            base.OnDraw(time, delta);
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            if (!(Mesh is null))
                Mesh.Dispose();
            MeshRenderer.Dispose();
            DiffuseSampler.Dispose();
        }
    }
}
