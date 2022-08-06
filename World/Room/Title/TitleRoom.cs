using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.Title
{
    internal class TitleRoom : IRoom
    {
        private static readonly Color backgroundColorTheme0 = Potato.ColorTheme1;
        private static readonly Color backgroundColorTheme1 = Potato.ColorTheme3;
        private TitleMenu titleMenu;
        private Texture2D backgroundTexture;
        public IController Controller { get; set; }
        public OpenCloseState RoomState => throw new NotImplementedException();

        public TitleRoom()
        {
            titleMenu = new TitleMenu();

            int gameWidth = Potato.Game.GraphicsDevice.Viewport.Width;
            int gameHeight = Potato.Game.GraphicsDevice.Viewport.Height;
            
            backgroundTexture = new Texture2D(
                graphicsDevice: Potato.Game.GraphicsDevice,
                width: gameWidth,
                height: gameHeight,
                mipmap: false,
                format: SurfaceFormat.Color);
            Color[] colors = new Color[gameWidth * gameHeight];
            for (int row = 0; row < gameHeight; row++)
            {
                float ratio = row / gameHeight;
                Color color = (ratio * backgroundColorTheme0).Add((1 - ratio) * backgroundColorTheme1);
                for (int col = 0; col < gameWidth; col++)
                    colors[col + row * gameWidth] = color;
            }
            backgroundTexture.SetData(colors);

            titleMenu.Position = new Vector2(
                x: (gameWidth - titleMenu.Size.Width) / 2,
                y: (gameHeight - titleMenu.Size.Height) / 2);
        }
        
        public void CloseRoom()
        {
            throw new NotImplementedException();
        }

        public void Draw(Matrix? transformMatrix)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: backgroundTexture, position: Vector2.Zero, color: Color.White);
            spriteBatch.End();

            titleMenu.Draw(transformMatrix: transformMatrix);
        }

        public void OpenRoom()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            titleMenu.Update(gameTime: gameTime);
        }
    }
}
