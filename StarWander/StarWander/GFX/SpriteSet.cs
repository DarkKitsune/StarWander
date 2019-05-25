using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class SpriteSet : INameable
    {
        /// <summary>
        /// Get the name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the texture of the SpriteSet
        /// </summary>
        public Texture2D? Texture { get; private set; }

        /// <summary>
        /// The collection of sprites
        /// </summary>
        private Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Get an collection of the sprites
        /// </summary>
        public Dictionary<string, Sprite>.ValueCollection Collection => Sprites.Values;

        public Sprite this[string name]
        {
            get
            {
                return GetSprite(name);
            }
        }

        private SpriteSet()
        {
            Name = "";
            Texture = null;
        }

        public SpriteSet(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;
        }

        /// <summary>
        /// Create a SpriteSet from a directory
        /// </summary>
        /// <param name="name">SpriteSet name</param>
        /// <param name="path">Directory path</param>
        /// <param name="recursive">Whether to use a recursive file search</param>
        public static SpriteSet FromDirectory(string name, string path, bool recursive, bool bufferSpriteEdges)
        {
            // Create bitmaps from .png files
            var pngs = Directory.GetFiles(path, "*.png", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var bitmaps = new Bitmap[pngs.Length];
            for (var i = 0; i < pngs.Length; i++)
            {
                var file = pngs[i];
                var fullPath = Path.Combine(path, file);
                bitmaps[i] = (Bitmap)Image.FromFile(fullPath);
            }

            // Create an atlas of the bitmaps
            var atlas = TextureAtlas.CreateBitmapAtlas(bitmaps, 2, out var regions, bufferSpriteEdges);
            // Dispose the bitmaps
            foreach (var bitmap in bitmaps)
                bitmap.Dispose();
            // Create a texture from the atlas
            var tex = Texture2D.FromBitmap($"{name} Texture", atlas, true);
            // Dispose the atlas
            atlas.Dispose();

            // Create default shader program
            var program = ShaderProgram.Create($"{name} Shader Program");
            using (var vert = Content.LoadShader(Path.Combine("shaders", "sprites.vert"), ShaderType.Vertex))
            {
                using (var frag = Content.LoadShader(Path.Combine("shaders", "sprites.frag"), ShaderType.Fragment))
                {
                    program.AttachShaders(vert, frag);
                    program.Link();
                    program.DetachShaders(vert, frag);
                }
            }
            program.EnableBlockCamera();

            // Create a spriteset using the atlas texture
            var spriteSet = new SpriteSet(name, tex);
            // Add sprites to the spriteset using the bitmap file names and atlas regions
            for (var i = 0; i < pngs.Length; i++)
            {
                var spriteName = Path.GetFileNameWithoutExtension(pngs[i]);
                spriteSet.SetSprite(spriteName, program, regions[i]);
            }

            // Return the new SpriteSet
            return spriteSet;
        }

        /// <summary>
        /// Set a sprite in the SpriteSet
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <param name="leftTop">Top-left corner</param>
        /// <param name="size">Size</param>
        public Sprite SetSprite(string name, ShaderProgram shaderProgram, Vector2<int> leftTop, Vector2<int> size = default)
        {
            var sprite = new Sprite(name, this, shaderProgram, leftTop, size);
            Sprites[name] = sprite;
            return sprite;
        }

        /// <summary>
        /// Set a sprite in the SpriteSet
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <param name="rectangle">Sprite bounds</param>
        public Sprite SetSprite(string name, ShaderProgram shaderProgram, Rectangle rectangle)
        {
            var sprite = new Sprite(name, this, shaderProgram, rectangle);
            Sprites[name] = sprite;
            return sprite;
        }

        /// <summary>
        /// Get a sprite in the SpriteSet
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <exception cref="ArgumentException"></exception>
        public Sprite GetSprite(string name)
        {
            if (Sprites.TryGetValue(name, out var sprite))
                return sprite;
            throw new ArgumentException($"Sprite with name \"{name}\" doesn't exist in this SpriteSet");
        }
    }
}
