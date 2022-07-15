using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Menu
{
    internal class TextMenu : IMenu
    {
        private static BitmapFont font;
        private static readonly Color color = Color.Black;
        private readonly List<string> lines;
        public TextMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public void Apply()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Size.Width)
                {
                    lines.Add(currentLine.Trim());
                    currentLine = token.Value + " ";
                }
                else if (token.IsLast)
                {
                    currentLine += token.Value;
                    lines.Add(currentLine.Trim());
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: lines.Count * font.LineHeight);
        }
        public string Text { get; set; }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(
                    font: font,
                    text: line,
                    position: Position + new Vector2(0, index * font.LineHeight),
                    color: color);
        }
        public void Update(GameTime gameTime)
        {
        }
    }
}
