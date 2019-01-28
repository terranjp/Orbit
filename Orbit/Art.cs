using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Orbit
{
    static class Art
    {
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Circle { get; private set; }
        public static SpriteFont DebugText { get; set; }

        public static void Load(ContentManager content)
        {
            Circle = content.Load<Texture2D>("white_circle");
            DebugText = content.Load<SpriteFont>("DebugFont");
            Cursor = content.Load<Texture2D>("cursor");
            
        }

    }
}
