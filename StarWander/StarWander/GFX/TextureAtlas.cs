using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public static class TextureAtlas
    {
        private const int MaxDimension = 4096; // Atlas cannot have a side above this size

        /// <summary>
        /// Generate a new bitmap that is an atlas of other bitmaps
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="edgeBuffering"></param>
        /// <param name="regions"></param>
        /// <param name="bufferWithImageData"></param>
        /// <exception cref="ArgumentException"></exception>
        public static Bitmap CreateBitmapAtlas(Bitmap[] bitmaps, int edgeBuffering, out Rectangle[] regions, bool bufferWithImageData)
        {
            if (bitmaps.Length == 0)
                throw new ArgumentException("bitmaps must be longer than 0", nameof(bitmaps));

            var minimumSafeArea = bitmaps.Sum((e) => (e.Width + edgeBuffering * 2) * (e.Height + edgeBuffering * 2));
            var oneSide = (int)Math.Pow(2, Math.Ceiling(Math.Log2(Math.Sqrt(minimumSafeArea * 1.2))));

            // Choose initial atlas size
            var atlasWidth = oneSide;
            var atlasHeight = oneSide;

            // Attempt to find spots for atlas textures
            regions = new Rectangle[bitmaps.Length];
            for (var i = 0; i < regions.Length; i++)
                regions[i] = new Rectangle(0, 0, bitmaps[i].Width, bitmaps[i].Height);
            var increaseSide = 0;
            while (true)
            {
                // Look for spots for every texture
                var successful = true;
                var lastY = edgeBuffering;
                for (var i = 0; i < regions.Length; i++)
                {
                    var found = FindSpot(regions, i, new Vector2<int>(atlasWidth, atlasHeight), edgeBuffering, lastY);
                    if (!found)
                    {
                        successful = false;
                        break;
                    }
                    lastY = regions[i].Y;
                }
                if (successful)
                    break;

                // If unsuccessful, increase one of the atlas dimensions by 2x
                if (increaseSide == 0)
                {
                    if (atlasWidth * 2 > MaxDimension)
                        throw new Exception("Unable to create an atlas large enough to contain the given bitmaps");
                    else
                        atlasWidth *= 2;
                }
                else
                {
                    if (atlasHeight * 2 > MaxDimension)
                        throw new Exception("Unable to create an atlas large enough to contain the given bitmaps");
                    else
                        atlasHeight *= 2;
                }
                increaseSide = (increaseSide + 1) % 2;
            }

            // Create destination bitmap and lock its bits
            var destBitmap = new Bitmap(atlasWidth, atlasHeight);
            var destBits = destBitmap.LockBits(
                    new Rectangle(0, 0, atlasWidth, atlasHeight),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb
                );
            var destSize = destBits.Height * destBits.Stride;
            // Loop over every texture region
            for (var i = 0; i < regions.Length; i++)
            {
                var region = regions[i];
                var srcBitmap = bitmaps[i];
                // Lock the texture bitmap
                var srcBits = srcBitmap.LockBits(
                        new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb
                    );
                // Copy the texture to the destination bitmap
                unsafe
                {
                    var srcPtr = (byte*)srcBits.Scan0;
                    for (var y = 0; y < srcBitmap.Height; y++)
                    {
                        var destPtr = (byte*)(destBits.Scan0 + ((region.Y + y) * destBits.Stride + region.X * 4));
                        Buffer.MemoryCopy(srcPtr, destPtr, destSize, srcBits.Stride);
                        srcPtr += srcBits.Stride;

                        if (bufferWithImageData)
                        {
                            // Buffer left/right edges
                            for (var buff = 0; buff < edgeBuffering; buff++)
                            {
                                var leftBuffer = destPtr + (buff - edgeBuffering) * 4;
                                var leftImage = destPtr;
                                var rightBuffer = destPtr + destBits.Stride + buff * 4;
                                var rightImage = destPtr + destBits.Stride - 4;
                                leftBuffer[0] = leftImage[0];
                                leftBuffer[1] = leftImage[1];
                                leftBuffer[2] = leftImage[2];
                                leftBuffer[3] = leftImage[3];
                                rightBuffer[0] = rightImage[0];
                                rightBuffer[1] = rightImage[1];
                                rightBuffer[2] = rightImage[2];
                                rightBuffer[3] = rightImage[3];
                            }
                        }
                    }
                    if (bufferWithImageData)
                    {
                        // Buffer top/bottom
                        var start = (byte*)(destBits.Scan0 + (region.Y * destBits.Stride + region.X * 4));
                        var widthBytes = region.Width * 4;
                        var topBuffer = (void*)start;
                        var topImage = start + destBits.Stride;
                        var bottomBuffer = start + region.Height * destBits.Stride;
                        var bottomImage = start + (region.Height - 1) * destBits.Stride;
                        Buffer.MemoryCopy(topImage, topBuffer, widthBytes, widthBytes);
                        Buffer.MemoryCopy(bottomImage, bottomBuffer, widthBytes, widthBytes);
                    }
                }
                // Unlock the texture bitmap
                srcBitmap.UnlockBits(srcBits);
            }
            // Unlock the destination bitmap 
            destBitmap.UnlockBits(destBits);

            // Return destination bitmap
            return destBitmap;
        }

        private static bool FindSpot(
                Rectangle[] regions, int current, Vector2<int> dimensions, int minSeparation, int lastY
            )
        {
            var currentRegion = regions[current];
            for (var y = lastY; y < dimensions.Y - minSeparation - currentRegion.Height; y += 2)
                for (var x = minSeparation; x < dimensions.X - minSeparation - currentRegion.Width; x += 2)
                {
                    currentRegion.X = x;
                    currentRegion.Y = y;
                    regions[current] = currentRegion;
                    if (SpotValid(regions, current, minSeparation))
                    {
                        return true;
                    }
                    MoveOutside(regions, current, ref x, y);
                }
            return false;
        }

        private static void MoveOutside(Rectangle[] regions, int current, ref int x, int y)
        {
            for (var i = 0; i < current; i++)
            {
                var region = regions[i];
                if (x >= region.X && x < region.Right && y >= region.Y && y < region.Bottom)
                    x = region.Right;
            }
        }

        private static bool SpotValid(
                Rectangle[] regions, int current, int minSeparation
            )
        {
            var currentRegion = regions[current];
            for (var i = 0; i < current; i++)
                if (RegionsCollide(currentRegion, regions[i], minSeparation))
                    return false;
            return true;
        }

        private static bool RegionsCollide(Rectangle a, Rectangle b, int minSeparation)
        {
            return a.X < b.X + b.Width + minSeparation
                && a.X + a.Width + minSeparation > b.X
                && a.Y < b.Y + b.Height + minSeparation
                && a.Y + a.Height + minSeparation > b.Y;
        }
    }
}
