using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNACTL
{
    public class XNACore
    {
        public static ContentManager Content;
        public static SpriteBatch Batch;
        public static GraphicsDevice Graphics;

        public static void Initialize(ContentManager content, GraphicsDevice graphics)
        {
            Content = content;
            Batch = new SpriteBatch(graphics);
            Graphics = graphics;
        }
    }
}
