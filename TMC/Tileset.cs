﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using TMC.Imaging;

namespace TMC
{
    public class Tileset
    {
        private Pixelmap baseImg;
        private Pixelmap[] tiles;

        // cache
        private Bitmap cachedImg;
        private int cachedImgWidth, cachedZoom;

        public Tileset(Pixelmap pixelmap)
        {
            this.baseImg = pixelmap;
            this.tiles = pixelmap.Tile();

            this.cachedImg = null;
            this.cachedImgWidth = -1;
            this.cachedZoom = 0;
        }

        ~Tileset()
        {
            if (cachedImg != null)
                cachedImg.Dispose();
            // nothing? Pixelmap's destructor handles stuff like that
        }

        public Pixelmap this[int index]
        {
            get { return tiles[(index >= tiles.Length ? 0 : index)]; }
            set { tiles[index] = value; }
        }

        public int[] CalculatePerfectWidths()
        {
            List<int> widths = new List<int>();
            for (int i = 1; i <= tiles.Length; i++)
            {
                if (tiles.Length % i == 0) widths.Add(i);
            }
            return widths.ToArray<int>();
        }

        public Size[] CalculatePerfectSizes()
        {
            int[] widths = CalculatePerfectWidths();

            Size[] result = new Size[widths.Length];
            for (int x = 0; x < result.Length; x++)
            {
                result[x] = new Size(widths[x], tiles.Length / widths[x]);
            }
            return result;
        }

        public Bitmap Draw(int width, int zoom)
        {
            if (width == cachedImgWidth && cachedImg != null && zoom == cachedZoom)
            {
                return cachedImg;
            }
            else
            {
                int height = tiles.Length / width;
                if (tiles.Length % width > 0) height++;

                Bitmap bmp = new Bitmap(width * 8 * zoom, height * 8 * zoom);
                Graphics g = Graphics.FromImage(bmp);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        g.DrawImage(this[x + y * width].Draw(zoom), x * 8 * zoom, y * 8 * zoom, 8 * zoom, 8 * zoom);
                    }
                }
                g.Dispose();

                cachedImg = bmp;
                cachedImgWidth = width;
                cachedZoom = zoom;
                return bmp;
            }
        }

        public int Length
        {
            get { return tiles.Length; }
        }
    }
}
