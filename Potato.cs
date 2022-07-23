using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
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
        private OptionMenu optionMenu;
        private KeyboardController keyboard;
        private float timer = 10.0f;

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
            //menu = new ContainerMenu(
            //    components: new List<IMenu>()
            //    {
            //        new TextMenu(text: "Hello! My name is Andrew, I am testing the menu out.", align: Alignment.Center, width: 512),
            //        new TextMenu(text: "This is purely just a test to verify everything is working the way that I want.", align: Alignment.Center, width: 512),
            //        new DividerMenu(width: 256),
            //        new SelectMenu(text: "This is a select menu. NOOOOIIIICE", align: Alignment.Center, width: 256),
            //        new SelectMenu(text: "Blah blah blah blah", align: Alignment.Center, width: 256),
            //        new SliderMenu(width: 512, fill: 0.25f),
            //        new SliderMenu(width: 512, fill: 0.75f),
            //        new RadioMenu(
            //            options: new List<string>(){ "Option1", "Option2"},
            //            align: Alignment.Center, width: 512, selected: 1),
            //        new TypingMenu(width: 512),
            //        new SelectImageMenu(texture: Content.Load<Texture2D>("potato")),
            //        new TypingMenu(width: 128),
            //        new ImageMenu(texture: Content.Load<Texture2D>("potato"))
            //    },
            //    align: Alignment.Center);
            //menu.Position = new Vector2(x: 128, y: 32);
            //menu.Controller = keyboard;
            //menu.OpenMenu();

            OptionMenuSave optionMenuSave = new OptionMenuSave()
            {
                MasterVolume = 0.5f,
                MusicVolume = 0.1f,
                EffectVolume = 1.0f,
                DisplayMode = DisplayModeType.Windowed,
                ActivateKey = Keys.Enter,
                BackKey = Keys.Back,
                LeftKey = Keys.Left,
                RightKey = Keys.Right,
                UpKey = Keys.Up,
                DownKey = Keys.Down
            };
            optionMenu = new OptionMenu(save: optionMenuSave);
            optionMenu.Position = new Vector2(x: (gameWidth - optionMenu.Size.Width) / 2, y: 32);
            optionMenu.Controller = keyboard;
            optionMenu.OpenMenu();
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
            //menu.Update(gameTime);
            keyboard.Update(gameTime);
            optionMenu.Update(gameTime);
            //keyboard.CollectKeys = true;
            //var keysPressed = keyboard.KeysPressed;
            //test.Append(keysPressed.Where((x) => x.Key != Keys.Back).Select((x) => x.Character).ToArray());
            //var removeAmount = keysPressed.Where((x) => x.Key == Keys.Back).Count();
            //test.Remove(test.Length - removeAmount, removeAmount);

            //keysPressed.Clear();
            //Console.WriteLine($"Current Text: {test}");

            //timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (timer < 0)
            //{
            //    timer = 10.0f;
            //    menu.CloseMenu();
            //}
            base.Update(gameTime);
        }

        private StringBuilder test = new StringBuilder();

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.RosyBrown);
            
            // TODO: Add your drawing code here
            //menu.Draw(spriteBatch);
            optionMenu.Draw(spriteBatch);
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
