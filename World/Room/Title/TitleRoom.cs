﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potato.Room;
using Potato.World.Menu;
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
        private RoomStateChanger roomStateChanger;
        public ISelectable EngineEditorSelect => titleMenu.EngineEditorSelect;
        public OpenCloseState RoomState => roomStateChanger.RoomState;
        public IController Controller { get => titleMenu.Controller; set => titleMenu.Controller = value; }

        public TitleRoom(OptionMenu optionMenu)
        {
            int gameWidth = Potato.Game.GraphicsDevice.Viewport.Width;
            int gameHeight = Potato.Game.GraphicsDevice.Viewport.Height;
            
            titleMenu = new TitleMenu(optionMenu: optionMenu);
            titleMenu.Position = new Vector2(
                x: (gameWidth - titleMenu.Size.Width) / 2,
                y: (gameHeight - titleMenu.Size.Height) / 2);

            backgroundTexture = new Texture2D(
                graphicsDevice: Potato.Game.GraphicsDevice,
                width: gameWidth,
                height: gameHeight,
                mipmap: false,
                format: SurfaceFormat.Color);
            Color[] colors = new Color[gameWidth * gameHeight];
            for (int row = 0; row < gameHeight; row++)
            {
                float ratio = (float)row / gameHeight;
                Color color = (ratio * backgroundColorTheme1).Add((1 - ratio) * backgroundColorTheme0);
                for (int col = 0; col < gameWidth; col++)
                    colors[col + row * gameWidth] = color;
            }
            backgroundTexture.SetData(colors);

            roomStateChanger = new RoomStateChanger();
        }

        public void CloseRoom()
        {
            titleMenu.CloseMenu();
            roomStateChanger.CloseRoom();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;

            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: backgroundTexture, position: Vector2.Zero, color: Color.White);
            spriteBatch.End();

            titleMenu.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void OpenRoom()
        {
            titleMenu.OpenMenu();
            roomStateChanger.OpenRoom();
        }

        public void Update(GameTime gameTime)
        {
            titleMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }

        public void SoftReset()
        {
            titleMenu.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void HardReset()
        {
            titleMenu.HardReset();
            roomStateChanger.HardReset();
        }
    }
}
