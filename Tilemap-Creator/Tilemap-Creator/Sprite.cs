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
    public class Sprite : IDisposable
    {
        Bitmap image;
        BitmapData imageData;
        bool locked = false;

        int width, height;
        int[] pixels;
        Color[] palette;

        public Sprite(int width, int height, int colors = 256)
        {
            this.width = width;
            this.height = height;
            pixels = new int[width * height];

            palette = new Color[colors];

            image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public Sprite(int width, int height, Color[] palette)
        {
            this.width = width;
            this.height = height;
            pixels = new int[width * height];

            this.palette = palette;

            image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        // creates a new sprite for use with the GBA/NDS from a regular image
        // note
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
            using (var image = source.ChangeFormat(PixelFormat.Format24bppRgb))
            {
                // grab pixel data
                var imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                var buffer = new byte[width * height  * 3];
                Marshal.Copy(imageData.Scan0, buffer, 0, width * height * 3);

                // generate image data, including a palette
                // palette generation is the slow part,
                // but use of a int list is not bad even for a huge image
                var colors = new List<int>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel data for (x, y)
                        // and quantize it
                        var i = (x + y * width) * 3;
                        var r = (buffer[i] / 8) * 8;
                        var g = (buffer[i + 1] / 8) * 8;
                        var b = (buffer[i + 2] / 8) * 8;
                        var c = (r << 16) | (g << 8) | b;

                        // try to add to palette
                        if (!colors.Contains(c)) colors.Add(c);

                        // set color data
                        pixels[x + y * width] = colors.IndexOf(c);
                    }
                }

                // create palette now
                palette = new Color[colors.Count];
                for (int i = 0; i < colors.Count; i++)
                    palette[i] = Color.FromArgb(colors[i]);

                // !!! don't forget to unlock source
                image.UnlockBits(imageData);
            }

            // fills the image cache for the first time
            Lock(); Unlock();
        }

        public void Dispose()
        {
            image?.Dispose();
        }

        /// <summary>
        /// Locks the Sprite's cache and prepares it for pixel writing.
        /// </summary>
        public void Lock()
        {
            if (locked) return;
            locked = true;

            // lock bits
            imageData = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
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

        /// <summary>
        /// Returns the pixel at the given position. The Sprite does not need to be locked.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The x-coordinate of the pixel to retrieve.</param>
        /// <returns>A Color from the palette for the data at the given position.</returns>
        public Color GetPixel(int x, int y)
        {
            return palette[pixels[x + y * width]];
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
    }
}
