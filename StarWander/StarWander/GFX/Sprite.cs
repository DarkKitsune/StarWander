using System;
using System.Drawing;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class Sprite : DrawableAsset
    {
        /// <summary>
        /// SpriteSet this Sprite is attached to
        /// </summary>
        public SpriteSet SpriteSet { get; private set; }
        /// <summary>
        /// Top-left corner of the sprite in the SpriteSet's texture
        /// </summary>
        public Vector2<int> TopLeft { get; private set; }
        /// <summary>
        /// Size of the sprite in the SpriteSet's texture
        /// </summary>
        public Vector2<int> Size { get; private set; }
        /// <summary>
        /// Top-left corner of the sprite in the SpriteSet's texture, in texture coordinates
        /// </summary>
        public Vector2<float> TopLeftCoord { get; private set; }
        /// <summary>
        /// Size of the sprite in the SpriteSet's texture, in texture coordinates
        /// </summary>
        public Vector2<float> SizeCoord { get; private set; }

        /// <summary>
        /// Create a sprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spriteSet"></param>
        /// <param name="topLeft"></param>
        /// <param name="size"></param>
        public Sprite(string name, SpriteSet spriteSet, ShaderProgram shaderProgram, Vector2<int> topLeft, Vector2<int> size) : base(name, shaderProgram)
        {
            if (spriteSet.Texture == null)
                throw new NullReferenceException("SpriteSet's texture is null");
            SpriteSet = spriteSet;
            TopLeft = topLeft;
            Size = size;
            TopLeftCoord = new Vector2<float>(
                    (float)TopLeft.X / spriteSet.Texture.Dimensions[0].X,
                    1f - (float)TopLeft.Y / spriteSet.Texture.Dimensions[0].Y
                );
            SizeCoord = new Vector2<float>(
                    (float)Size.X / spriteSet.Texture.Dimensions[0].X,
                    (float)Size.Y / -spriteSet.Texture.Dimensions[0].Y
                );
        }

        /// <summary>
        /// Create a sprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spriteSet"></param>
        /// <param name="rectangle"></param>
        public Sprite(string name, SpriteSet spriteSet, ShaderProgram shaderProgram, Rectangle rectangle) : base(name, shaderProgram)
        {
            if (spriteSet.Texture == null)
                throw new NullReferenceException("SpriteSet's texture is null");
            SpriteSet = spriteSet;
            TopLeft = rectangle.GetLeftTop();
            Size = rectangle.GetSize();
            TopLeftCoord = new Vector2<float>(
                    (float)TopLeft.X / spriteSet.Texture.Dimensions[0].X,
                    1f - (float)TopLeft.Y / spriteSet.Texture.Dimensions[0].Y
                );
            SizeCoord = new Vector2<float>(
                    (float)Size.X / spriteSet.Texture.Dimensions[0].X,
                    (float)Size.Y / -spriteSet.Texture.Dimensions[0].Y
                );
        }
    }
}
