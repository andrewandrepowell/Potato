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
        public Alignment Align { get => Alignment.Left; set { } }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { } }

        public ImageMenu(Texture2D texture)
        {
            this.texture = texture;
            size = new Size2(texture.Width, texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: texture, position: Position, color: Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
