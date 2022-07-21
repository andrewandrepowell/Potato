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
        private Texture2D texture = null;
        public Alignment Align { get => Alignment.Left; set { } }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get; set; } = Size2.Empty;
        public Texture2D Texture { get; set; } = null;

        public void ApplyChanges()
        {
            texture = Texture;
            Size = new Size2(texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            if (texture != null)
            {
                spriteBatch.Begin(transformMatrix: transformMatrix);
                spriteBatch.Draw(texture: texture, position: Position, color: Color.White);
                spriteBatch.End();
            }
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
