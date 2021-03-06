﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TMC.Core
{
    public class Tileset
    {
        public const int TileSize = 8;

        /// <summary>
        /// Represents raw pixel data.
        /// </summary>
        public struct Tile
        {
            /// <summary>
            /// The pixel data.
            /// </summary>
            private int[] pixels;

            /// <summary>
            /// Initializes a new instance of the <see cref="Tile"/> struct by copying pixels from the specified array.
            /// </summary>
            /// <param name="pixels">The pixels to be copied.</param>
            public Tile(int[] pixels)
            {
                if (pixels == null)
                    throw new ArgumentNullException(nameof(pixels));

                if (pixels.Length != 64)
                    throw new ArgumentException("Expected 64 bits of pixel data.", nameof(pixels));

                this.pixels = (int[])pixels.Clone();
            }

            /// <summary>
            /// Gets or sets the specified pixel value.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int this[int x, int y]
            {
                get
                {
                    if (x < 0 || x >= 8)
                        throw new ArgumentOutOfRangeException(nameof(x));

                    if (y < 0 || y >= 8)
                        throw new ArgumentOutOfRangeException(nameof(y));

                    if (pixels == null)
                    {
                        return 0;
                    }

                    return pixels[x + y * 8];
                }
                set
                {
                    if (x < 0 || x >= 8)
                        throw new ArgumentOutOfRangeException(nameof(x));

                    if (y < 0 || y >= 8)
                        throw new ArgumentOutOfRangeException(nameof(y));

                    if (pixels == null)
                    {
                        pixels = new int[64];
                    }

                    pixels[x + y * 8] = value;
                }
            }

            /// <summary>
            /// Determines whether this tile value is equivalent to the specified tile value with the specified flipping.
            /// </summary>
            /// <param name="other">The tile to compare to.</param>
            /// <param name="flipX">Determines whether <paramref name="other"/> is to be flipped from left to right.</param>
            /// <param name="flipY">Determines whether <paramref name="other"/> is to be flipped from top to bottom.</param>
            /// <returns><c>true</c> if the tiles are equivalent; otherwise, <c>false</c>.</returns>
            public unsafe bool CompareTo(ref Tile other, bool flipX = false, bool flipY = false)
            {
                fixed (int* src = &pixels[0])
                fixed (int* dst = &other.pixels[0])
                {
                    for (int srcY = 0; srcY < 8; srcY++)
                    {
                        for (int srcX = 0; srcX < 8; srcX++)
                        {
                            var dstX = flipX ? (7 - srcX) : srcX;
                            var dstY = flipY ? (7 - srcY) : srcY;

                            if (src[srcX + srcY * 8] != dst[dstX + dstY * 8])
                                return false;
                        }
                    }
                }

                return true;
            }

            /// <summary>
            /// Gets the pixel data.
            /// </summary>
            public int[] Pixels => pixels ?? (pixels = new int[64]);
        }

        private Tile[] tiles;
        private Color[] palette;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class from the specified source image.
        /// </summary>
        /// <param name="source">The source image.</param>
        public Tileset(Bitmap source)
        {
            if (source.Width < 8 || source.Height < 8)
                throw new ArgumentException("Image must be at least 8x8 pixels.", nameof(source));

            if (source.Width / 8 * source.Height / 8 > 0x400)
                throw new ArgumentException("Image is too large, ensure it has no more than 0x400 (1024) tiles.", nameof(source));

            // Create the tiles
            var width = source.Width / 8;
            var height = source.Height / 8;
            tiles = new Tile[width * height];

            // Copy image data from source
            using (var fb = FastBitmap.FromImage(source))
            {
                if ((source.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
                {
                    // Copy the colors from the existing palette
                    switch (source.PixelFormat)
                    {
                        case PixelFormat.Format1bppIndexed:
                            palette = new Color[1 << 1];
                            break;

                        case PixelFormat.Format4bppIndexed:
                            palette = new Color[1 << 4];
                            break;

                        case PixelFormat.Format8bppIndexed:
                            palette = new Color[1 << 8];
                            break;

                        default:
                            throw new ArgumentException("Unsupported image format.", nameof(source));
                    }

                    source.Palette.Entries.CopyTo(palette, 0);

                    // Copy the tiles
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            ref var tile = ref tiles[x + y * width];

                            for (int j = 0; j < 8; j++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    // Find the color index
                                    var index = palette.IndexOf(Color.FromArgb(fb.Bits[(x * 8 + i) + (y * 8 + j) * fb.Width]));
                                    if (index < 0)
                                        throw new IndexOutOfRangeException();

                                    // Copy to the tile
                                    tile[i, j] = index;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Create a new palette while copying tiles
                    var colors = new List<Color>();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            ref var tile = ref tiles[x + y * width];

                            for (int j = 0; j < 8; j++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    var color = Color.FromArgb(fb.Bits[(x * 8 + i) + (y * 8 + j) * fb.Width]).Quantize();

                                    var index = colors.IndexOf(color);
                                    if (index < 0)
                                    {
                                        colors.Add(color);
                                        index = colors.Count - 1;
                                    }

                                    tile[i, j] = index;
                                }
                            }
                        }
                    }

                    // Set the palette
                    palette = colors.ToArray();
                }
            }
        }

        /// <summary>
        /// Initialzies a new instance of the <see cref="Tileset"/> with the specified tile array.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        protected Tileset(Tile[] tiles, Color[] palette)
        {
            this.tiles = tiles;
            this.palette = palette;
        }

        #region Methods

        /// <summary>
        /// Gets the specified tile.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <returns></returns>
        public ref Tile this[int index] => ref tiles[index];

        /// <summary>
        /// Creates a new <see cref="Tileset"/> from the specified source image.
        /// </summary>
        /// <param name="bmp">The source image.</param>
        /// <param name="allowFlipping">Determines whether tile flipping is permitted.</param>
        public static (Tileset Tileset, Tilemap Tilemap) Create(Bitmap bmp, bool allowFlipping)
        {
            var width = bmp.Width / 8;
            var height = bmp.Height / 8;

            // Create initial tileset (with all tiles)
            var tileset = new Tileset(bmp);

            // Create empty tilemap
            var tilemap = new Tilemap(width, height);

            // Define the first tile
            tilemap[0] = new Tilemap.Tile();

            // Scan the tileset for repeated tiles
            var tiles = new List<Tile> { tileset[0] };
            var current = 1;

            for (int i = 1; i < tileset.Length; i++)
            {
                ref var tile = ref tileset[i];

                // The current tile
                var index = current;
                var flipX = false;
                var flipY = false;

                // Compare the tile against all unique tiles
                for (int j = 0; j < tiles.Count; j++)
                {
                    var other = tiles[j];

                    // Test tile configurations
                    if (tile.CompareTo(ref other, false, false))
                    {
                        index = j;
                        break;
                    }
                    else if (allowFlipping)
                    {
                        if (tile.CompareTo(ref other, true, false))
                        {
                            index = j;
                            flipX = true;
                            break;
                        }

                        if (tile.CompareTo(ref other, false, true))
                        {
                            index = j;
                            flipY = true;
                            break;
                        }

                        if (tile.CompareTo(ref other, true, true))
                        {
                            index = j;
                            flipX = true;
                            flipY = true;
                            break;
                        }
                    }
                }

                // Update the tilemap
                tilemap[i] = new Tilemap.Tile((short)index, flipX, flipY);

                // Update the tileset
                if (index >= current)
                {
                    tiles.Add(tile);
                    current++;
                }
            }

            // The process is now finished
            return (new Tileset(tiles.ToArray(), tileset.palette), tilemap);
        }

        /// <summary>
        /// Creates a new <see cref="FastBitmap"/> representing all tiles.
        /// </summary>
        /// <param name="columns">The number of columns in a single row of tiles.</param>
        /// <returns></returns>
        public FastBitmap ToImage(int columns)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns));

            var rows = (tiles.Length / columns) + (tiles.Length % columns > 0 ? 1 : 0);
            var fb = new FastBitmap(columns * 8, rows * 8);

            for (int i = 0; i < tiles.Length; i++)
            {
                // Get the destination
                var x = i % columns;
                var y = i / columns;

                // Get the tile to draw
                ref var tile = ref tiles[i];

                // Draw the tile
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        fb.SetPixel(x * 8 + k, y * 8 + j, palette[tile[k, j]]);
                    }
                }
            }

            return fb;
        }

        public void SaveBMP(string filename, int columns)
        {
            var width = columns * 8;
            var height = (tiles.Length / columns + (tiles.Length % columns > 0 ? 1 : 0)) * 8;

            // Creates a pixel buffer for the tiles
            var pixels = new int[width * height];
            for (int i = 0; i < tiles.Length; i++)
            {
                ref var tile = ref tiles[i];

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        pixels[(x + i % columns * 8) + (y + i / columns * 8) * width] = tile[x, y];
                    }
                }
            }

            using (var bw = new BinaryWriter(File.Create(filename)))
            {
                if (palette.Length <= 16)
                {
                    var rowSize = ((4 * width + 31) / 32) * 4;
                    var pixelSize = rowSize * height;
                    var paddingSize = rowSize % 4;

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

                        bw.Write(color.B);
                        bw.Write(color.G);
                        bw.Write(color.R);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x += 2)
                        {
                            bw.Write((byte)((pixels[x + y * width] << 4) | pixels[x + 1 + y * width]));
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
                else if (palette.Length <= 256)
                {
                    var rowSize = ((8 * width + 31) / 32) * 4;
                    var pixelSize = rowSize * height;
                    var paddingSize = rowSize % 4;

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

                        bw.Write(color.B);
                        bw.Write(color.G);
                        bw.Write(color.R);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x++)
                        {
                            ref var tile = ref tiles[x + y * columns];
                            bw.Write((byte)tile[0, 0]);
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }
                else
                {
                    var rowSize = ((24 * width + 31) / 32) * 4;
                    var pixelSize = rowSize * height;
                    var paddingSize = rowSize % 4;

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

                    // Pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // Copy colors for this row
                        for (int x = 0; x < width; x++)
                        {
                            var color = palette[pixels[x + y * width]];
                            bw.Write(color.B);
                            bw.Write(color.G);
                            bw.Write(color.R);
                        }

                        // Pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }
            }
        }

        public void SaveGBA(string filename)
        {
            if (palette.Length <= 16)
            {
                File.WriteAllBytes(filename, BitDepth.Encode4(tiles));
            }
            else if (palette.Length <= 256)
            {
                File.WriteAllBytes(filename, BitDepth.Encode8(tiles));
            }
            else
            {
                throw new InvalidOperationException("Tileset has too many colors to save.");
            }
        }

        /// <summary>
        /// Returns an array of column values that will result in a perfect tileset.
        /// </summary>
        /// <returns></returns>
        public int[] GetPerfectColumns()
        {
            var columns = new List<int>();

            for (int i = 1;i <= tiles.Length; i++)
            {
                if (tiles.Length % i == 0) columns.Add(i);
            }

            return columns.ToArray();
        }

        /// <summary>
        /// Reduces the number of colors to no more than the amount specified.
        /// </summary>
        /// <param name="colorCount">The maximum number of colors.</param>
        public void ReduceColors(int colorCount)
        {
            if (palette == null || palette.Length <= colorCount) return;

            // Create the quantizer and add the palette
            var quantizer = new OctreeQuantizer();
            quantizer.AddColors(palette);

            // Create the reduced palette
            var reducedPalette = quantizer.GetPalette(colorCount);

            // Update all tiles to reflect the reduced colors
            for (int i = 0; i < tiles.Length; i++)
            {
                ref var tile = ref tiles[i];

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        // Get the original color
                        var pixel = palette[tile[x, y]];

                        // Get the closets match from the quantizer
                        var index = quantizer.GetPaletteIndex(pixel);

                        // Update the pixel
                        tile[x, y] = index;
                    }
                }
            }

            // Replace the old palette
            palette = reducedPalette.ToArray();
        }

        /// <summary>
        /// Swaps the colors by the order specified in a new palette.
        /// </summary>
        /// <param name="newColors">Specifies the order of the new palette.</param>
        public void SwapColors(Color[] newColors)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of tiles.
        /// </summary>
        public int Length => tiles.Length;

        /// <summary>
        /// Gets the palette.
        /// </summary>
        public Color[] Palette => palette;

        #endregion
    }
}
