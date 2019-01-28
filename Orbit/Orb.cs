using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Orbit
{
    internal class Circle
    {
        public Circle()
        {
            Image = Art.Circle;
        }

        protected Color color = Color.White;
        protected float Scale => Diameter / Image.Bounds.Width;

        public Texture2D Image;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Center => new Vector2(Position.X + Radius, Position.Y + Radius);

        public float density = 1;
        public float Diameter = 50;
        public float Radius => Diameter / 2;
        public float Mass => Area * density;

        public float Area
        {
            get { return Radius * Radius * (float)Math.PI; }
            set { Diameter = (float)Math.Sqrt(4 * value / (float)Math.PI); }
        }

        public bool Intersects(Rectangle rectangle)
        {
            var corners = new[]
            {
                new Point(rectangle.Top, rectangle.Left),
                new Point(rectangle.Top, rectangle.Right),
                new Point(rectangle.Bottom, rectangle.Right),
                new Point(rectangle.Bottom, rectangle.Left)
            };

            foreach (Point corner in corners)
            {
                if (ContainsPoint(corner))
                    return true;
            }

            // next we want to know if the left, top, right or bottom edges overlap
            if (Center.X - Diameter > rectangle.Right || Center.X + Diameter < rectangle.Left)
                return false;

            if (Center.Y - Diameter > rectangle.Bottom || Center.Y + Diameter < rectangle.Top)
                return false;

            return true;
        }

        public bool Intersects(Orb orb)
        {
            var center0 = new Vector2(orb.Center.X, orb.Center.Y);
            var center1 = new Vector2(Center.X, Center.Y);
            return Vector2.Distance(center0, center1) < Radius + orb.Radius;
        }

        public bool Contains(Orb orb)
        {
            if (ContainsPoint(orb.Center.ToPoint()))
            {
                var center0 = new Vector2(orb.Center.X, orb.Center.Y);
                var center1 = new Vector2(Center.X, Center.Y);
                return Vector2.Distance(center0, center1) + orb.Radius > Radius;
            }
            return false;
        }

        public bool IntersectsVertical(Rectangle rectangle)
        {
            return Center.Y + Radius > GameRoot.Instance.gameArea.Bottom || Center.Y - Radius < GameRoot.Instance.gameArea.Top;
        }

        public bool IntersectsHorizontal(Rectangle rectangle)
        {
            return Center.X + Radius > GameRoot.Instance.gameArea.Right || Center.X - Radius < GameRoot.Instance.gameArea.Left;
        }

        public bool ContainsPoint(Point point)
        {
            var vector2 = new Vector2(point.X - Center.X, point.Y - Center.Y);
            return vector2.Length() <= Radius;
        }
    }

    internal abstract class Orb : Circle
    {
        public bool Alive = true;

        public abstract void Update();

        public Vector2 Size => new Vector2(Diameter, Diameter);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture: Image,
                position: Position,
                sourceRectangle: null,
                color: color,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: 0f);
        }
    }

    internal class PlayerOrb : Orb
    {
        public MouseState mouseState;
        public MouseState lastMouseState;
        private bool leftMousePressed = false;
        private static PlayerOrb instance;
        public float bulletArea;

        public static PlayerOrb Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerOrb();
                return instance;
            }
        }

        private PlayerOrb()
        {
            Position = new Vector2(GameRoot.Instance.gameArea.Center.X - Radius, GameRoot.Instance.gameArea.Center.Y - Radius);
            Diameter = 50;
            Image = Art.Circle;
            Position = GameRoot.ScreenSize / 2;
        }

        public void Reset()
        {
            Position = new Vector2(GameRoot.Instance.gameArea.Center.X - Radius, GameRoot.Instance.gameArea.Center.Y - Radius);
            Diameter = 50;
            Image = Art.Circle;
            Position = GameRoot.ScreenSize / 2;
        }

        private void Shoot()
        {
            var aimDirection = Input.GetMouseAimDirection();

            float aimAngle = Utils.VectorToAngle(aimDirection);
            float bulletMagnitude = Velocity.LengthSquared() + 1.0f;
            bulletArea = Area * 0.05f;

            float bulletDiameter = Utils.DiameterFromCircleArea(bulletArea);

            Vector2 bulletVelocity = Utils.FromPolar(aimAngle, bulletMagnitude);

            // TODO: Bullet does not instantiate perfectly on edge of players orb
            Vector2 bulletPosition = new Vector2(
                x: Center.X + (Radius + bulletDiameter / 2 + 1) * (float)Math.Cos(aimAngle),
                y: Center.Y + (Radius + bulletDiameter / 2 + 1) * (float)Math.Sin(aimAngle)
                );

            Bullet bullet = new Bullet(instance, bulletPosition, bulletVelocity, bulletArea);
            OrbManager.AddBullet(bullet);

            Area = Area - bullet.Area;
            Velocity -= bulletVelocity / 10;
        }

        public override void Update()
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                leftMousePressed = true;

            if (mouseState.LeftButton == ButtonState.Released && leftMousePressed)
            {
                leftMousePressed = false;
                Shoot();
            }

            if (IntersectsVertical(GameRoot.Instance.gameArea))
                Velocity.Y = -Velocity.Y;

            if (IntersectsHorizontal(GameRoot.Instance.gameArea))
                Velocity.X = -Velocity.X;

            Position += Velocity;
        }
    }

    internal class Bullet : Orb
    {
        public Bullet(PlayerOrb playerOrb, Vector2 startPosition, Vector2 startVelocity, float newArea)
        {
            Image = Art.Circle;
            Area = newArea;
            Position = new Vector2(x: startPosition.X - Radius, y: startPosition.Y - Radius);
            Velocity = startVelocity;
        }

        public override void Update()
        {
            if (Diameter <= 0)
                Alive = false;

            if (IntersectsVertical(GameRoot.Instance.gameArea))
                Velocity.Y = -Velocity.Y;

            if (IntersectsHorizontal(GameRoot.Instance.gameArea))
                Velocity.X = -Velocity.X;

            if (Radius > PlayerOrb.Instance.Radius)
                color = Color.Red;
            else
                color = Color.Blue;

            Position += Velocity;
        }
    }
}