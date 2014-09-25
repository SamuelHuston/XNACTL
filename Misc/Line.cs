﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNACTL
{
    public struct Line
    {
        static Vector2 origin = new Vector2(0, 0.5f);
        static SpriteBatch Batch = XNACore.Batch;

        Rectangle rect;
        float angle;

        public Line(int xPos, int yPos, float xDim, float yDim, int scale)
        {
            if (xDim < 0)
                angle = (float)Math.Atan(yDim / xDim) + MathHelper.Pi;
            else
                angle = (float)Math.Atan(yDim / xDim);

            rect = new Rectangle((int)((xPos + 0.5) * scale), (int)((yPos + 0.5) * scale), (int)(new Vector2(xDim, yDim).Length() * scale), 1);
        }

        public void Initialize(int xPos, int yPos, float xDim, float yDim, int scale)
        {
            if (xDim < 0)
                angle = (float)Math.Atan(yDim / xDim) + MathHelper.Pi;
            else
                angle = (float)Math.Atan(yDim / xDim);

            rect = new Rectangle((int)((xPos + 0.5) * scale), (int)((yPos + 0.5) * scale), (int)(new Vector2(xDim, yDim).Length() * scale), 1);
        }

        public void Draw(Texture2D texture)
        {
            Batch.Draw(texture, rect, null, Color.Red, angle, origin, SpriteEffects.None, 0);
        }
    }
}
