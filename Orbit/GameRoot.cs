using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbit
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameRoot : Game
    {
        public Texture2D whiteRectangle;
        public string debugText;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }

        public Rectangle gameArea;

        public static Vector2 ScreenSize
        {
            get { return new Vector2(Viewport.Width, Viewport.Height); }
        }

        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            Content.RootDirectory = "Content";
            Instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            int width = GraphicsDevice.Viewport.Bounds.X;
            int height = GraphicsDevice.Viewport.Bounds.Y;
            gameArea = new Rectangle(100, 100, GraphicsDevice.Viewport.Bounds.Right - 200, GraphicsDevice.Viewport.Bounds.Bottom - 200);

            PlayerOrb.Instance.mouseState = Mouse.GetState();
            PlayerOrb.Instance.Position = new Vector2(gameArea.Center.X - PlayerOrb.Instance.Radius, gameArea.Center.Y - PlayerOrb.Instance.Radius);
            OrbManager.playerOrb = PlayerOrb.Instance;       
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
            Art.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update();
            OrbManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            debugText = "";
            debugText += $"\n Orb x: {PlayerOrb.Instance.Center.X} y: {PlayerOrb.Instance.Center.Y}";
            debugText += $"\n Csr x: {Input.MousePosition.X + 12} y: {Input.MousePosition.Y + 12}";

            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin(SpriteSortMode.Texture);

            OrbManager.Draw(spriteBatch);
            spriteBatch.Draw(Art.Cursor, Input.MousePosition, Color.White);
            spriteBatch.Draw(whiteRectangle, gameArea, Color.Black);
            spriteBatch.DrawString(Art.DebugText, debugText, new Vector2(5, 5), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}