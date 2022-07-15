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
        private readonly List<float> widthOffsets;
        public TextMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            widthOffsets = new List<float>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public void ApplyChanges()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            widthOffsets.Clear();
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Size.Width)
                {
                    string newLine = currentLine.Trim();
                    lines.Add(newLine);
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
                    widthOffsets.Add(widthOffset);
                    currentLine = token.Value + " ";
                }
                else if (token.IsLast)
                {
                    currentLine += token.Value;
                    string newLine = currentLine.Trim();
                    lines.Add(newLine);
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
                    widthOffsets.Add(widthOffset);
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
        public Alignment Align { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line, float widthOffset) in lines
                .Select((line, index) => (index, line))
                .Zip(widthOffsets, (tuple, widthOffset) => (tuple.index, tuple.line, widthOffset)))
                spriteBatch.DrawString(
                    font: font,
                    text: line,
                    position: Position + new Vector2(widthOffset, index * font.LineHeight),
                    color: color);
        }
        public void Update(GameTime gameTime)
        {
        }
    }
}
