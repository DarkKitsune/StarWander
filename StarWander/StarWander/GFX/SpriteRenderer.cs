using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using VulpineLib.Util;

namespace StarWander.GFX
{
    public class SpriteRenderer : Renderer
    {
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
        }

        /// <summary>
        /// Construct a SpriteRenderer
        /// </summary>
        /// <param name="name"></param>
        public SpriteRenderer(string name) : base(name)
        {
            VertexAttributes = VertexAttributes.Create($"{Name} Vertex Attributes");
            DiffuseSampler = Sampler.Create($"{Name} SpriteRenderer Diffuse Sampler");
        }

        ~SpriteRenderer()
        {
            Dispose();
        }

        private VertexAttributes VertexAttributes;
        private Sampler DiffuseSampler;
        private Sprite? LastSprite;
        private SpriteSet? LastSpriteSet;

        /// <summary>
        /// Begin drawing sprites
        /// </summary>
        protected override void OnBegin()
        {
            // Bind shader program, VAO, and sampler
            VertexAttributes.Bind();
            DiffuseSampler.BindDiffuse();
            // Reset previous Sprite/SpriteSet cache
            LastSprite = null;
            LastSpriteSet = null;
            State.CheckError();
        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="transform"></param>
        /// <param name="color"></param>
        /// <param name="blendType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void DrawSprite(Sprite sprite, Matrix4 transform, VulpineLib.Util.Color color, BlendType blendType)
        {
            Assert();

            if (!Drawing)
                throw new InvalidOperationException("Not drawing; Begin() must be called first");
            var spriteSet = sprite.SpriteSet;
            // If this Sprite's SpriteSet is not the same as the cached previous one, then
            //  update the cache to the new one and bind the new one's texture
            if (spriteSet != LastSpriteSet)
            {
                if (spriteSet.Texture == null)
                    throw new NullReferenceException($"Sprite's SpriteSet ({sprite.SpriteSet.Name}) has a null Texture");
                spriteSet.Texture.BindDiffuse();
                LastSpriteSet = spriteSet;
            }
            // If this Sprite is not the same as the cached previous one, then
            //  update the cache to the new one and set the relevant shader uniforms
            if (sprite != LastSprite!)
            {
                sprite.ShaderProgram.Bind();
                sprite.ShaderProgram.SetTextureSource(sprite.TopLeftCoord, sprite.SizeCoord);
                LastSprite = sprite;
            }
            // Bind the BlendType
            blendType.Bind();
            // Set the model color
            sprite.ShaderProgram.SetModelColor(color);
            // Set the model matrix
            sprite.ShaderProgram.SetModelMatrix(transform);
            // Draw 4 vertices
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            State.CheckError();
        }

        /// <summary>
        /// Draw a sprite at a position with a size
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="blendType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void DrawSprite(Sprite sprite, Vector3<float> position, Vector2<float> size, VulpineLib.Util.Color color, BlendType blendType)
        {
            Assert();

            if (!Drawing)
                throw new InvalidOperationException("Not drawing; Begin() must be called first");
            var spriteSet = sprite.SpriteSet;
            // If this Sprite's SpriteSet is not the same as the cached previous one, then
            //  update the cache to the new one and bind the new one's texture
            if (spriteSet.Texture != LastSpriteSet?.Texture)
            {
                if (spriteSet.Texture == null)
                    throw new NullReferenceException($"Sprite's SpriteSet ({sprite.SpriteSet.Name}) has a null Texture");
                spriteSet.Texture.BindDiffuse();
                LastSpriteSet = spriteSet;
            }
            // If this Sprite is not the same as the cached previous one, then
            //  update the cache to the new one and set the relevant shader uniforms
            if (sprite != LastSprite!)
            {
                sprite.ShaderProgram.Bind();
                sprite.ShaderProgram.SetTextureSource(sprite.TopLeftCoord, sprite.SizeCoord);
                LastSprite = sprite;
            }
            // Bind the BlendType
            blendType.Bind();
            // Set the model color
            sprite.ShaderProgram.SetModelColor(color);
            // Set the model position and scale
            sprite.ShaderProgram.SetPositionScale(
                    position,
                    new Vector3<float>(size.X, size.Y, 0f)
                );
            // Draw 4 vertices
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            State.CheckError();
        }

        /// <summary>
        /// Draw a sprite spanning a rectangle
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        /// <param name="blendType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void DrawSprite(Sprite sprite, VulpineLib.Geometry.Rectangle rectangle, VulpineLib.Util.Color color, BlendType blendType)
        {
            DrawSprite(
                    sprite,
                    new Vector3<float>(
                            rectangle.Center,
                            0f
                        ),
                    new Vector2<float>(
                            rectangle.Width,
                            rectangle.Height
                        ),
                    color,
                    blendType
                );
        }

        /// <summary>
        /// Finish drawing sprites
        /// </summary>
        protected override void OnEnd()
        {
            ShaderProgram.Unbind();
            Texture2D.UnbindDiffuse();
            VertexAttributes.Unbind();
            Sampler.UnbindDiffuse();
            BlendType.Unbind();
            State.CheckError();
        }

        protected override void OnDispose()
        {
            VertexAttributes.Dispose();
            DiffuseSampler.Dispose();
        }
    }
}
