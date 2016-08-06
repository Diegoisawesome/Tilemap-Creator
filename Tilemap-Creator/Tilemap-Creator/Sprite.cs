﻿using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TMC
{
    [Flags]
    public enum SpriteFormat
    {
        BMP = 0x0000,
        PNG = 0x0001,
    }

    public class Sprite : IDisposable
    {
        Bitmap image;
        BitmapData imageData;
        bool locked = false;

        int width, height;
        int[] pixels;
        Color[] palette;

        public Sprite(int width, int height, Color[] palette)
        {
            this.width = width;
            this.height = height;
            pixels = new int[width * height];

            this.palette = palette;

            image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        // creates a new sprite for use with the GBA/NDS from a regular image
        //? should there be an option to limit palette size...
        public Sprite(Bitmap source)
        {
            // create image data
            width = source.Width;
            height = source.Height;
            pixels = new int[width * height];

            // init cache
            image = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            // get image data from source
            // create new Bitmap holding source but in 24bpp format
            // TODO: preserve indexed bitmaps
            Console.WriteLine($"format: {source.PixelFormat}");
            using (var source24 = source.ChangeFormat(PixelFormat.Format24bppRgb))
            {
                // grab pixel data
                var sourceData = source24.LockBits(PixelFormat.Format24bppRgb);
                var buffer = new byte[width * height * 3];
                Marshal.Copy(sourceData.Scan0, buffer, 0, width * height * 3);

                // create a palette, copy pixel data
                // if the image is already indexed, that palette should be used
                if ((source.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
                {
                    // TODO: copy source palette                    
                    foreach (var color in source.Palette.Entries)
                    {
                        var r = color.R / 8 * 8;
                        var g = color.G / 8 * 8;
                        var b = color.B / 8 * 8;

                        Console.WriteLine("color = [{0}, {1}, {2}]", r, g, b);
                    }
                }

                var colors = new List<int>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel data for (x, y)
                        // and quantize it
                        var i = (x + y * width) * 3;
                        var r = buffer[i] / 8 * 8;
                        var g = buffer[i + 1] / 8 * 8;
                        var b = buffer[i + 2] / 8 * 8;
                        var c = (r << 16) | (g << 8) | b;

                        // update palette as needed
                        var index = colors.IndexOf(c);
                        if (index < 0)
                        {
                            colors.Add(c);
                            index = colors.Count - 1;
                        }
                        pixels[x + y * width] = index;
                    }
                }

                // create palette now
                palette = new Color[colors.Count];
                for (int i = 0; i < colors.Count; i++)
                    palette[i] = Color.FromArgb(colors[i]);


                // !!! don't forget to unlock source
                source24.UnlockBits(sourceData);
            }

            // fills the image cache for the first time
            Lock(); Unlock();
        }

        // creates a sprite with data from a region within the source
        public Sprite(Sprite source, Rectangle region)
        {
            // create pixels
            width = region.Width;
            height = region.Height;
            pixels = new int[width * height];

            // copy palette
            palette = source.palette;

            // init cache
            image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // copy image data from source
            for (int y = 0; y < region.Height; y++)
            {
                for (int x = 0; x < region.Width; x++)
                {
                    int x2 = x + region.X;
                    int y2 = y + region.Y;

                    if (x2 >= source.width || y2 >= source.height) continue;

                    pixels[x + y * width] = source.pixels[x2 + y2 * source.width];
                }
            }

            // cache drawing
            Lock(); Unlock();
        }

        public void Dispose()
        {
            image?.Dispose();
        }

        public void Save(string filename, SpriteFormat format)
        {
            // selects the best bitdepth
            int bitDepth = 24;
            if (palette.Length <= 256) bitDepth = 8;
            if (palette.Length <= 16) bitDepth = 4;

            Save(filename, format, bitDepth);
        }

        public void Save(string filename, SpriteFormat format, int bitDepth)
        {
            if (locked) throw new Exception("Cannot save a locked Sprite!");

            switch (format)
            {
                case SpriteFormat.BMP:
                    SaveBitmap(filename, bitDepth);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void SaveBitmap(string filename, int bitDepth)
        {
            // https://en.wikipedia.org/wiki/BMP_file_format

            // some calculations
            int rowSize = ((bitDepth * width + 31) / 32) * 4;   // number of bytes per row
            int pixelSize = rowSize * height;                   // number of pixels in bytes
            int paddingSize = rowSize % 4;                      // number of extra bytes per row

            Console.WriteLine("Saving Bitmap:");
            Console.WriteLine("- bitdepth={0}", bitDepth);
            Console.WriteLine("- rowSize={0}", rowSize);
            Console.WriteLine("- pixelSize={0}", pixelSize);
            Console.WriteLine("- paddingSize={0}", paddingSize);

            using (var bw = new BinaryWriter(File.Create(filename)))
            {
                // TODO: Indexed images may be compressed using RLE of Huffma
                if (bitDepth == 4)
                {
                    // Bitmap file header
                    bw.Write((ushort)0x4D42);               // 'BM'
                    bw.Write(pixelSize + (16 * 4) + 54);    // filesize = header + color table + pixel data
                    bw.Write(0x293A);                       // embed a friendly message
                    bw.Write(54 + (16 * 4));                // offset of pixel data

                    // BITMAPINFOHEADER
                    bw.Write(40);               // header size = 40 bytes
                    bw.Write(width);            // width in pixels
                    bw.Write(height);           // height in pixels
                    bw.Write((ushort)1);        // 1 color plane
                    bw.Write((ushort)4);        // 8 bpp
                    bw.Write(0);                // no compression
                    bw.Write(pixelSize);        // size of raw data + padding
                    bw.Write(2835);             // print resoltion of image (~72 dpi)
                    bw.Write(2835);             //
                    bw.Write(16);               // color table size, 16 because MUST be 2^n
                    bw.Write(0);                // all colors are important

                    // color table
                    for (int i = 0; i < 16; i++)
                    {
                        var color = (i < palette.Length ? palette[i] : Color.Black);

                        bw.Write(color.R);
                        bw.Write(color.G);
                        bw.Write(color.B);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x += 2)
                        {
                            var left = pixels[x + y * width];
                            var right = pixels[x + y * width + 1];
                            var pixel = (left << 4) | right;
                            bw.Write((byte)pixel);
                        }

                        // include the last pixel in odd number widths
                        if (width % 2 != 0)
                        {
                            bw.Write((byte)(pixels[(width - 1) + y * width] << 4));
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }

                if (bitDepth == 8)
                {
                    // Bitmap file header
                    bw.Write((ushort)0x4D42);               // 'BM'
                    bw.Write(pixelSize + (256 * 4) + 54);   // filesize = header + color table + pixel data
                    bw.Write(0x293A);                       // embed a friendly message
                    bw.Write(54 + (256 * 4));               // offset of pixel data

                    // BITMAPINFOHEADER
                    bw.Write(40);               // header size = 40 bytes
                    bw.Write(width);            // width in pixels
                    bw.Write(height);           // height in pixels
                    bw.Write((ushort)1);        // 1 color plane
                    bw.Write((ushort)8);        // 8 bpp
                    bw.Write(0);                // no compression
                    bw.Write(pixelSize);        // size of raw data + padding
                    bw.Write(2835);             // print resoltion of image (~72 dpi)
                    bw.Write(2835);             //
                    bw.Write(256);              // color table size, 256 because MUST be 2^n
                    bw.Write(0);                // all colors are important

                    // color table
                    for (int i = 0; i < 256; i++)
                    {
                        var color = (i < palette.Length ? palette[i] : Color.Black);

                        bw.Write(color.R);
                        bw.Write(color.G);
                        bw.Write(color.B);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x++)
                        {
                            bw.Write((byte)pixels[x + y * width]);
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }

                if (bitDepth == 24)
                {
                    // Bitmap file header
                    bw.Write((ushort)0x4D42);   // 'BM'
                    bw.Write(pixelSize + 54);   // filesize = header + pixel data
                    bw.Write(0x293A);           // embed a friendly message
                    bw.Write(54);               // offset of pixel data

                    // BITMAPINFOHEADER
                    bw.Write(40);               // header size = 40 bytes
                    bw.Write(width);            // width in pixels
                    bw.Write(height);           // height in pixels
                    bw.Write((ushort)1);        // 1 color plane
                    bw.Write((ushort)24);       // 24 bpp
                    bw.Write(0);                // no compression
                    bw.Write(pixelSize);        // size of raw data + padding
                    bw.Write(2835);             // print resoltion of image (~72 dpi)
                    bw.Write(2835);             //
                    bw.Write(0);                // empty color table
                    bw.Write(0);                // all colors are important

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x++)
                        {
                            var color = palette[pixels[x + y * width]];
                            bw.Write(color.R);
                            bw.Write(color.G);
                            bw.Write(color.B);
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Locks the Sprite's cache and prepares it for pixel writing.
        /// </summary>
        public void Lock()
        {
            if (locked) return;
            locked = true;

            // lock bits
            imageData = image.LockBits(PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Unlocks the Sprite's pixel data, updating the cached Bitmap.
        /// </summary>
        public void Unlock()
        {
            if (!locked) return;
            locked = false;

            // update image cache, unlock bits
            var buffer = new byte[width * height * 3];
            for (int i = 0; i < width * height; i++)
            {
                var color = palette[pixels[i]];

                buffer[i * 3] = color.R;
                buffer[i * 3 + 1] = color.G;
                buffer[i * 3 + 2] = color.B;
            }

            Marshal.Copy(buffer, 0, imageData.Scan0, buffer.Length);
            image.UnlockBits(imageData);
        }

        // TODO: we can use Compare to help override Equals

        /// <summary>
        /// Returns whether the given <c>Sprite</c> is equivalent to this <c>Sprite</c>.
        /// </summary>
        /// <param name="other">The other <c>Sprite</c> to compare.</param>
        /// <returns>Whether the given Sprite is equivalent to this.</returns>
        public bool Compare(Sprite other)
        {
            if (other.width != width || other.height != height) return false;

            // probably faster than the below method
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] != other.pixels[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns whether the given <c>Sprite</c> is equivalent to this <c>Sprite</c>.
        /// </summary>
        /// <param name="other">The other <c>Sprite</c> to compare.</param>
        /// <param name="flipX">Whether this <c>Sprite</c> should be flipped on the x-axis during comparison.</param>
        /// <param name="flipY">Whether this <c>Sprite</c> should be flipped on the y-axis during comparison.</param>
        /// <returns>Whether the given Sprite is equivalent to this.</returns>
        public bool Compare(Sprite other, bool flipX, bool flipY)
        {
            if (other.width != width || other.height != height) return false;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int x2 = flipX ? (width - x - 1) : x;
                    int y2 = flipY ? (height - y - 1) : y;

                    int indexThis = x2 + y2 * width;
                    int indexOther = x + y * width;

                    if (other.pixels[indexOther] != pixels[indexThis]) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the pixel at the given position. The Sprite does not need to be locked.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The x-coordinate of the pixel to retrieve.</param>
        /// <returns>An index within the palette for the pixel at the given position..</returns>
        public int GetPixel(int x, int y)
        {
            return pixels[x + y * width];
        }

        public void SetPixel(int x, int y, int paletteIndex)
        {
            if (!locked) throw new Exception("Sprite not locked!");

            pixels[x + y * width] = paletteIndex;
        }

        /// <summary>
        /// Sets every pixel to the first in the palette.
        /// </summary>
        public void Clear()
        {
            if (!locked) throw new Exception("Sprite not locked!");

            for (int i = 0; i < width * height; i++)
            {
                pixels[i] = 0;
            }
        }

        public void SwapColors(int color1, int color2, bool updateImage)
        {
            if (!locked) throw new Exception("Sprite not locked!");

            // move colors around in palette
            var temp = palette[color1];
            palette[color1] = palette[color2];
            palette[color2] = temp;

            // update image data only if told to
            if (updateImage)
            {
                for (int i = 0; i < width * height; i++)
                {
                    if (pixels[i] == color1) pixels[i] = color2;
                    else if (pixels[i] == color2) pixels[i] = color1;
                }
            }
        }

        public bool Locked
        {
            get { return locked; }
        }

        public Color[] Palette
        {
            get { return palette; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        // ease of use conversion:
        public static implicit operator Image(Sprite s)
        {
            return s.image;
        }

        public static implicit operator Bitmap(Sprite s)
        {
            return s.image;
        }
    }

    public static class BitmapExtensions
    {
        /// <summary>
        /// Creates a new Bitmap from the given Bitmap with a given PixelFormat.
        /// </summary>
        /// <param name="bmp">The source Bitmap to copy.</param>
        /// <param name="newFormat">The new PixelFormat for the Bitmap.</param>
        /// <returns>A new Bitmap with the given PixelFormat.</returns>
        public static Bitmap ChangeFormat(this Bitmap bmp, PixelFormat newFormat)
        {
            // convert a Bitmap to Format24bppRgb
            if (bmp.PixelFormat == newFormat)
                return new Bitmap(bmp);

            Bitmap result = null;
            Graphics gfx = null;
            try
            {
                // create new bitmap with desired format
                result = new Bitmap(bmp.Width, bmp.Height, newFormat);
                gfx = Graphics.FromImage(result);

                // copy image to newly formatted bitmap
                Rectangle bounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
                gfx.DrawImage(bmp, bounds, bounds, GraphicsUnit.Pixel);

                // guess that works
            }
            finally
            {
                gfx?.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Locks a Bitmap into system memory.
        /// </summary>
        public static BitmapData LockBits(this Bitmap bmp, PixelFormat format)
        {
            return bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, format);
        }
    }
}
