using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class TypingMenu : IMenu
    {
        private bool applyChanges = false;
        private Texture2D fieldTexture = null;
        private static readonly Color fieldColor = Potato.ColorTheme0;
        private KeyboardController keyboardController = null;
        public Alignment Align { get; set; } = Alignment.Left;
        public IController Controller { get => keyboardController; set => keyboardController = value as KeyboardController; }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public string Text { get; set; } = "";

        public void ApplyChanges() => applyChanges = true;

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            applyChanges = false;
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
