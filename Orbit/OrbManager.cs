using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Orbit
{
   
    internal static class OrbManager
    {
        private static bool isUpdating;
        public static PlayerOrb playerOrb;
        public static List<Bullet> bullets = new List<Bullet>();
        public static List<Bullet> addedBullets = new List<Bullet>();

        public static void AddBullet(Bullet bullet)
        {
            if (!isUpdating)
                bullets.Add(bullet);
            else
                addedBullets.Add(bullet);
        }

        public static void Update(GameTime gameTime)
        {
            isUpdating = true;
            playerOrb.Update();
            int bulletCount = bullets.Count();

            for (int i = 0; i < bulletCount; i++)
            {
                bullets[i].Update();
                if (bullets[i].Intersects(PlayerOrb.Instance))
                    Physics.Collide(PlayerOrb.Instance, bullets[i]);

                for (int j = i + 1; j < bulletCount; j++)
                    if (bullets[i].Intersects(bullets[j]))
                        Physics.Collide(bullets[i], bullets[j]);
            }
            bullets.RemoveAll(s => s.Alive == false);

            if (!playerOrb.Alive)
            {
                addedBullets.Clear();
                bullets.Clear();
                playerOrb.Reset();
                GameRoot.Instance.debugText = "GAME OVER";
            }
               
            isUpdating = false;

            foreach (var bullet in addedBullets)
                bullets.Add(bullet);
            addedBullets.Clear();       
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bullet in bullets)
                bullet.Draw(spriteBatch);
            playerOrb.Draw(spriteBatch);
        }
    }
}