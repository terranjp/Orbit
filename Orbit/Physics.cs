using Microsoft.Xna.Framework;


namespace Orbit
{
    static class Physics
    {
        public static void Collide(Orb orb1, Orb orb2)
        {
            Orb masterOrb;
            Orb littleOrb;

            if (orb1.Radius == orb2.Radius)
            {
                if (orb1.Velocity.LengthSquared() > orb2.Velocity.LengthSquared())
                {
                    masterOrb = orb1;
                    littleOrb = orb2;
                }
                else
                {
                    masterOrb = orb2;
                    littleOrb = orb1;
                }
            } else if (orb1.Radius > orb2.Radius)
            {
                masterOrb = orb1;
                littleOrb = orb2;
            }
            else
            {
                masterOrb = orb2;
                littleOrb = orb1;
            }

            if (masterOrb.Contains(littleOrb))
            {
                masterOrb.Velocity = (masterOrb.Velocity * (float)masterOrb.Mass + littleOrb.Velocity * (float)littleOrb.Mass) / (float)(masterOrb.Mass + littleOrb.Mass);
                masterOrb.Area = masterOrb.Area + littleOrb.Area;
                littleOrb.Alive = false;
            }
            else
            {
                float areaDelta = 0.1f * littleOrb.Area;
                littleOrb.Area -= areaDelta;
                masterOrb.Area += areaDelta;
                masterOrb.Velocity = (masterOrb.Velocity * (float)masterOrb.Mass + littleOrb.Velocity * areaDelta) / (float)(masterOrb.Mass + areaDelta);
            }
        }
    }
}
