using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Potato
{
    public class Potato : Game
    {
        public static Game Game { get; private set; }
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Menu menu;

        public Potato()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Game = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            menu = new MenuManager();
            TextMenu text0 = new TextMenu();
            TextMenu text1 = new TextMenu();
            text0.Text = "Hello! My name is Andrew, I am testing the menu out.";
            text0.Width = 512;
            text1.Text = "This is purely just a test to verify everything is working the way that I want.";
            text1.Width = 256;
            menu.Items.Add(text0);
            menu.Items.Add(text1);
            menu.Apply();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            menu.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            menu.Draw(_spriteBatch);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
