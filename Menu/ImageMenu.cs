using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class ImageMenu : IMenu
    {
        private Texture2D texture;
        private Size2 size;
        private VisibilityStateChanger state = new VisibilityStateChanger();
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get => state.State; }

        public ImageMenu(Texture2D texture)
        {
            this.texture = texture;
            size = new Size2(texture.Width, texture.Height);
        }

        public void OpenMenu() => state.OpenMenu();

        public void CloseMenu() => state.CloseMenu();
        
        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: texture, position: Position, color: state.Alpha * Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            state.Update(gameTime);
        }
    }
}
