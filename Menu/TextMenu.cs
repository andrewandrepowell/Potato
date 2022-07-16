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
        private readonly List<(string, float, float)> items;
        public TextMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            items = new List<(string, float, float)>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public void ApplyChanges()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            items.Clear();
            int heightIndex = 0;
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Size.Width)
                {
                    string newLine = currentLine.Trim();
                    float widthOffset = 0;
                    switch (Align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (Size.Width - font.MeasureString(newLine).Width) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = Size.Width - font.MeasureString(newLine).Width;
                            break;
                    }
                    float heightOffset = heightIndex * font.LineHeight;
                    items.Add((newLine, widthOffset, heightOffset));
                    heightIndex++;
                    currentLine = token.Value + " ";
                }
                else if (token.IsLast)
                {
                    currentLine += token.Value;
                    string newLine = currentLine.Trim();
                    float widthOffset = 0;
                    switch (Align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (Size.Width - font.MeasureString(newLine).Width) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = Size.Width - font.MeasureString(newLine).Width;
                            break;
                    }
                    float heightOffset = heightIndex * font.LineHeight;
                    items.Add((newLine, widthOffset, heightOffset));
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: items.Count * font.LineHeight);
        }
        public string Text { get; set; }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public Alignment Align { get; set; }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((string line, float widthOffset, float heightOffset) in items)
                spriteBatch.DrawString(
                    font: font,
                    text: line,
                    position: Position + new Vector2(widthOffset, heightOffset),
                    color: color);
        }
        public void Update(GameTime gameTime)
        {
        }
    }
}
