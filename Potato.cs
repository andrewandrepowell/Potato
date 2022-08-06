using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;
using Potato.World.Room.Title;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace Potato
{
    public class Potato : Game
    {
        public static Game Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static readonly Color ColorTheme0 = Color.White;
        public static readonly Color ColorTheme1 = Color.Yellow;
        public static readonly Color ColorTheme2 = new Color(r: 63, g: 67, b: 52, alpha: 100);
        public static readonly Color ColorTheme3 = Color.Black;
        public const string OptionSaveFileName = "option.save";
        private const int gameWidth = 1280; // 720p
        private const int gameHeight = 720; // 720p

        private TitleRoom titleRoom;
        private KeyboardController keyboard;

        public Potato()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
            Game = this;
#if DEBUG
            AllocConsole();
#endif
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Graphics.PreferredBackBufferWidth = gameWidth;
            Graphics.PreferredBackBufferHeight = gameHeight;
            Graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            keyboard = new KeyboardController();
            titleRoom = new TitleRoom();
            titleRoom.Controller = keyboard;
            titleRoom.Keyboard = keyboard;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboard.Update(gameTime: gameTime);
            titleRoom.Update(gameTime: gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.RosyBrown);

            titleRoom.Draw();
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
