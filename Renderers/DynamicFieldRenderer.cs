using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNACTL
{
    public class DynamicFieldRenderer
    {
        Texture2D Tile;
        Color[] PalletizedColors;
        int Width;
        int Height;
        double[,] Data;
        SpriteBatch Batch = XNACore.Batch;

        public DynamicFieldRenderer(Field f)
        {
            Data = f.Data;
            Width = f.Width;
            Height = f.Height;
            Tile = XNACore.Content.Load<Texture2D>("tile");

            PalletizedColors = new Color[256];

            for (int i = 0; i < 256; i++)
            {
                float c = ((float)i / 256) + 0.2f;
                PalletizedColors[i] = new Color(0, 0, c, c);
            }
        }

        public void Draw(float Scale)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (Data[x, y] > 0.00001)
                        Batch.Draw(Tile, new Vector2(x, y) * Scale, null, PalletizedColors[(int)Data[x, y] * 256], 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
