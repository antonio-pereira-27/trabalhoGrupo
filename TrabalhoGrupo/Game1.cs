using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoGrupo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        Ground ground;
        CameraSF cameraSF;
        Texture2D heightMap;
        Texture2D groundTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            heightMap = Content.Load<Texture2D>("lh3d1");
            groundTexture = Content.Load<Texture2D>("asphaltTexture");
            ground = new Ground(GraphicsDevice, heightMap, groundTexture);
            cameraSF = new CameraSF(GraphicsDevice, new Vector3(64, 5, 64), 0f, 0f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            cameraSF.Update(gameTime, keyboard, mouse, ground);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            ground.Draw(GraphicsDevice, cameraSF);

            base.Draw(gameTime);
        }
    }
}
