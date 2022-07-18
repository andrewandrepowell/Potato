using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Potato.Menu;
using System;
using System.Linq;
using System.Text;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace Potato
{
    public class Potato : Game
    {
        public static Game Game { get; private set; }
        public static readonly Color ColorTheme0 = Color.White;
        public static readonly Color ColorTheme1 = Color.Yellow;
        public static readonly Color ColorTheme2 = new Color(r: 63, g: 67, b: 52, alpha: 200);
        public static readonly Color ColorTheme3 = Color.Black;
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
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
            Game = this;
#if DEBUG
            AllocConsole();
#endif
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = gameWidth;
            graphics.PreferredBackBufferHeight = gameHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            keyboard = new KeyboardController();
            menu = new ContainerMenu();
            TextMenu text0 = new TextMenu();
            TextMenu text1 = new TextMenu();
            DividerMenu divider0 = new DividerMenu();
            SelectMenu select0 = new SelectMenu();
            SliderMenu slider0 = new SliderMenu();
            SelectMenu select1 = new SelectMenu();
            RadioMenu radio0 = new RadioMenu();
            TypingMenu typing0 = new TypingMenu();
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
            typing0.Size = new Size2(width: 512, height: 32);
            menu.Items.Add(text0);
            menu.Items.Add(text1);
            menu.Items.Add(divider0);
            menu.Items.Add(select0);
            menu.Items.Add(slider0);
            menu.Items.Add(select1);
            menu.Items.Add(radio0);
            menu.Items.Add(typing0);
            menu.Position = new Vector2(x: 256, y: 64);
            menu.Controller = keyboard;
            menu.Align = Alignment.Center;
            menu.ApplyChanges();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            menu.Update(gameTime);
            keyboard.Update(gameTime);
            //keyboard.CollectKeys = true;
            //var keysPressed = keyboard.KeysPressed;
            //test.Append(keysPressed.Where((x) => x.Key != Keys.Back).Select((x) => x.Character).ToArray());
            //var removeAmount = keysPressed.Where((x) => x.Key == Keys.Back).Count();
            //test.Remove(test.Length - removeAmount, removeAmount);

            //keysPressed.Clear();
            //Console.WriteLine($"Current Text: {test}");
            base.Update(gameTime);
        }

        private StringBuilder test = new StringBuilder();

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PowderBlue);
            
            // TODO: Add your drawing code here
            menu.Draw(spriteBatch);
            base.Draw(gameTime);
        }

#if DEBUG
        // https://gamedev.stackexchange.com/questions/45107/input-output-console-window-in-xna#:~:text=Right%20click%20your%20game%20in%20the%20solution%20explorer,tab.%20Change%20the%20Output%20Type%20to%20Console%20Application.
        // This opens a console window in the game.
        [DllImport("kernel32")]
        static extern bool AllocConsole();
#endif
    }
}
