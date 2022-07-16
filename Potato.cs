using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Potato.Menu;

namespace Potato
{
    public class Potato : Game
    {
        public static Game Game { get; private set; }
        private const int gameWidth = 1280; // 720p
        private const int gameHeight = 720; // 720p
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private ContainerMenu menu;
        KeyboardController keyboard;

        public Potato()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Game = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            keyboard = new KeyboardController();
            menu = new ContainerMenu();
            TextMenu text0 = new TextMenu();
            TextMenu text1 = new TextMenu();
            DividerMenu divider0 = new DividerMenu();
            SelectMenu select0 = new SelectMenu();
            SliderMenu slider0 = new SliderMenu();
            SelectMenu select1 = new SelectMenu();
            RadioMenu radio0 = new RadioMenu();
            text0.Text = "Hello! My name is Andrew, I am testing the menu out.";
            text0.Size = new Size2(width: 512, height: 0);
            text1.Text = "This is purely just a test to verify everything is working the way that I want.";
            text1.Size = new Size2(width: 512, height: 0);
            divider0.Size = new Size2(width: 512, height: 4);
            select0.Text = "This is a select menu. NOOOOIIIICE";
            select0.Size = new Size2(width: 512, height: 0);
            slider0.Fill = 0.5f;
            slider0.Size = new Size2(width: 512, height: 32);
            select1.Text = "This is another select item. WOo";
            select1.Size = new Size2(width: 512, height: 0);
            radio0.Options.Add("Hello");
            radio0.Options.Add("Dad");
            radio0.Options.Add("What is");
            radio0.Options.Add("OMMGG");
            radio0.Options.Add("This little piggy");
            radio0.Size = new Size2(width: 512, height: 0);
            menu.Items.Add(text0);
            menu.Items.Add(text1);
            menu.Items.Add(divider0);
            menu.Items.Add(select0);
            menu.Items.Add(slider0);
            menu.Items.Add(select1);
            menu.Items.Add(radio0);
            menu.Position = new Vector2(x: 256, y: 0);
            menu.Controller = keyboard;
            menu.Align = Alignment.Center;
            menu.ApplyChanges();
            graphics.PreferredBackBufferWidth = gameWidth;
            graphics.PreferredBackBufferHeight = gameHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            menu.Update(gameTime);
            keyboard.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            menu.Draw(spriteBatch);
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
