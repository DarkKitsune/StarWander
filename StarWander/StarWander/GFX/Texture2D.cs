using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class Texture2D : IGLObject
    {
        public bool Disposed { get; set; }

        /// <summary>
        /// The name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this object
        /// </summary>
        public GLHandle Handle { get; set; }

        public int Levels { get; private set; }

        /// <summary>
        /// The dimensions of each level
        /// </summary>
        public Vector2<int>[] Dimensions { get; private set; }

        /// <summary>
        /// The internal format of each level
        /// </summary>
        public PixelInternalFormat[] Formats { get; private set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private Texture2D()
        {
            Name = "";
            Dimensions = new Vector2<int>[] { };
            Formats = new PixelInternalFormat[] { };
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a texture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        /// <param name="levels"></param>
        private Texture2D(string name, int handle, int levels)
        {
            Name = name;
            Levels = levels;
            Dimensions = new Vector2<int>[levels];
            Formats = new PixelInternalFormat[levels];
            GL.BindTexture(TextureTarget.Texture2D, handle);
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Texture, handle, name.Length, name);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, levels - 1);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Create a texture
        /// </summary>
        /// <param name="name">The texture name</param>
        /// <param name="levels"></param>
        /// <returns>The new texture</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Texture2D Create(string name, int levels = 1)
        {
            if (levels < 0)
                throw new ArgumentOutOfRangeException(nameof(levels));
            return new Texture2D(name, GL.GenTexture(), levels);
        }

        /// <summary>
        /// Create a texture from bitmap data
        /// </summary>
        /// <param name="name">The texture name</param>
        /// <param name="size">The size of the image</param>
        /// <param name="bitmapData">The prepared bitmap data</param>
        /// <returns>The new texture</returns>
        public static unsafe Texture2D FromBitmapData(string name, Vector2<int> size, byte* bitmapData)
        {
            // Create texture that will be the destination for the image
            var texture2D = Create(name, 1);
            // Insert the image buffer into the texture
            texture2D.Image2D(size, 0, imageData: (IntPtr)bitmapData);
            return texture2D;
        }

        /// <summary>
        /// Create texture bitmap data from an image
        /// </summary>
        /// <param name="image">The Bitmap to use the image data of</param>
        /// <param name="premultiplied">Whether to premultiply the alpha</param>
        /// <param name="arrayHandleToFree"></param>
        /// <returns>The new bitmap data</returns>
        private static unsafe byte* GetBitmapData(Bitmap image, bool premultiplied, out GCHandle arrayHandleToFree)
        {
            var imageBufferArray = new byte[image.Height * image.Width * 4];
            arrayHandleToFree = GCHandle.Alloc(imageBufferArray, GCHandleType.Pinned);
            var imageBuffer = (byte*)arrayHandleToFree.AddrOfPinnedObject();
            TransferBitmapData(imageBuffer, image, premultiplied);
            return imageBuffer;
        }

        /// <summary>
        /// Copy and convert data from a Bitmap to a byte buffer
        /// </summary>
        /// <param name="imageBuffer">The destination byte buffer</param>
        /// <param name="image">The Bitmap to use the image data of</param>
        /// <param name="premultiplied">Whether to premultiply the alpha</param>
        private static unsafe void TransferBitmapData(byte* imageBuffer, Bitmap image, bool premultiplied)
        {
            // Lock image data to copy it
            var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );
            // Copy & reformat image into the image buffer
            for (var scanline = 0; scanline < bitmapData.Height; scanline++)
            {
                // Compute pointers for the beginning of the scanlines
                var destScanline = bitmapData.Height - 1 - scanline;
                var src = (byte*)bitmapData.Scan0 + scanline * bitmapData.Stride;
                var dest = imageBuffer + destScanline * bitmapData.Stride;
                // Loop over the pixels in the scanlines
                for (var pixel = 0; pixel < image.Width; pixel++)
                {
                    // Copy and format the pixel from source scanline to dest scanline
                    // Pixel must be converted from 32bpp ARGB to 32bpp RGBA
                    var b = *(src++);
                    var g = *(src++);
                    var r = *(src++);
                    var a = *(src++);
                    // Premultiply
                    if (premultiplied)
                    {
                        var premultiplyFactor = (float)a / 0xff;
                        r = (byte)(r * premultiplyFactor);
                        g = (byte)(g * premultiplyFactor);
                        b = (byte)(b * premultiplyFactor);
                    }
                    *(dest++) = r;
                    *(dest++) = g;
                    *(dest++) = b;
                    *(dest++) = a;
                }
            }
            image.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Create a texture from a bitmap
        /// </summary>
        /// <param name="name">The texture name</param>
        /// <param name="image">The Bitmap to use the image data of</param>
        /// <param name="premultiply">Whether to premultiply the alpha</param>
        /// <returns>The new texture</returns>
        public static Texture2D FromBitmap(string name, Bitmap image, bool premultiply = true)
        {
            unsafe
            {
                var tex = FromBitmapData(name, new Vector2<int>(image.Width, image.Height), GetBitmapData(image, premultiply, out var handle));
                handle.Free();
                return tex;
            }
        }

        /// <summary>
        /// Create a texture from an image file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path">The image file path</param>
        /// <param name="premultiplied"></param>
        /// <returns>The new texture</returns>
        public static Texture2D FromFile(string name, string path, bool premultiplied = true)
        {
            using (var image = Image.FromFile(path))
                return FromBitmap(name, (Bitmap)image, premultiplied);
        }

        /// <summary>
        /// Create a texture from an image stream
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream">The image stream</param>
        /// <param name="premultiplied"></param>
        /// <returns>The new texture</returns>
        public static Texture2D FromStream(string name, Stream stream, bool premultiplied = true)
        {
            using (var image = Image.FromStream(stream))
                return FromBitmap(name, (Bitmap)image, premultiplied);
        }

        /// <summary>
        /// Called during finalization
        /// </summary>
        ~Texture2D()
        {
            Dispose();
        }

        /// <summary>
        /// Bind the texture to the state
        /// </summary>
        /// <param name="target">Where to bind the texture</param>
        public void Bind(TextureTarget target = TextureTarget.Texture2D)
        {
            GL.BindTexture(target, Handle.Handle);
        }

        /// <summary>
        /// Bind the texture to a texture unit
        /// </summary>
        /// <param name="unit"></param>
        public void BindToTextureUnit(int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle.Handle);
        }

        /// <summary>
        /// Bind the texture to the diffuse texture unit
        /// </summary>
        public void BindDiffuse()
        {
            BindToTextureUnit((int)TextureUnits.Diffuse);
        }

        /// <summary>
        /// Bind the texture to the normal texture unit
        /// </summary>
        public void BindNormal()
        {
            BindToTextureUnit((int)TextureUnits.Normal);
        }

        /// <summary>
        /// Unbind all textures from the target in the state
        /// </summary>
        /// <param name="target">Where to unbind the textures from</param>
        public static void Unbind(TextureTarget target = TextureTarget.Texture2D)
        {
            GL.BindTexture(target, 0);
        }

        /// <summary>
        /// Unbind any texture from the texture unit in the state
        /// </summary>
        /// <param name="unit"></param>
        public static void UnbindFromTextureUnit(int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Unbind any texture from the diffuse texture unit
        /// </summary>
        public static void UnbindDiffuse()
        {
            UnbindFromTextureUnit((int)TextureUnits.Diffuse);
        }

        /// <summary>
        /// Unbind any texture from the normal texture unit
        /// </summary>
        public static void UnbindNormal()
        {
            UnbindFromTextureUnit((int)TextureUnits.Normal);
        }

        /// <summary>
        /// Define the texture's image contents for one level
        /// </summary>
        /// <param name="dimensions">Dimensions of the level</param>
        /// <param name="level">Which level</param>
        /// <param name="internalFormat">The internal image format</param>
        /// <param name="imageFormat">The source image pixel components</param>
        /// <param name="imageType">The source image pixel type</param>
        /// <param name="imageData">Pointer to the source image data, leave as zero for no source image data</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Image2D(
                Vector2<int> dimensions, int level = 0, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
                OpenTK.Graphics.OpenGL4.PixelFormat imageFormat = OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                PixelType imageType = PixelType.UnsignedByte, IntPtr imageData = default
            )
        {
            this.Assert();

            if (level < 0 || level >= Levels)
                throw new ArgumentOutOfRangeException(nameof(level));
            Bind(TextureTarget.Texture2D);
            Dimensions[level] = dimensions;
            Formats[level] = internalFormat;
            GL.TexImage2D(TextureTarget.Texture2D, level, internalFormat, dimensions.X, dimensions.Y, 0, imageFormat, imageType, imageData);
        }

        /// <summary>
        /// Sets the texture min/mag filters
        /// </summary>
        /// <param name="minFilter">Min filter</param>
        /// <param name="magFilter">Mag filter</param>
        public void SetFilter(TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            this.Assert();

            Bind(TextureTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteTexture(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
