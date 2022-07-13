using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal class SelectionMenu
    {
        
    }
    internal class SliderMenu
    {
        
    }
    internal class DividerMenu
    {

    }
    internal class TextMenu : IMenu, IDrawable
    {
        private static BitmapFont font;
        private static readonly Color color = Color.Black;
        private readonly List<string> lines;
        public void Apply()
        {
            if (Width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Width)
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
        }
        public TextMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Width = 0;
            Position = Vector2.Zero;
        }
        public string Text;
        public float Width { get; set; }
        public float Height => lines.Count * font.LineHeight;
        public Vector2 Position { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(
                    font: font, 
                    text: line, 
                    position: Position + new Vector2(0, index * font.LineHeight), 
                    color: color);
        }
    }
    internal class MenuManager : IMenu, IComponent
    {
        public MenuManager()
        {
            Items = new List<IMenu>();
            Position = Vector2.Zero;
        }
        public List<IMenu> Items { get; private set; }
        public Vector2 Position { get; set; }
        public float Width => Items.Select((item) => item.Width).Max();

        public float Height => Items.Select((item) => item.Height).Sum();

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IDrawable item in Items.OfType<IDrawable>())
                item.Draw(spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
            foreach (IUpdateable item in Items.OfType<IUpdateable>())
                item.Update(gameTime);
        }
        public void Apply()
        {
            float heightOffset = 0;
            foreach (IMenu item in Items)
            {
                item.Apply();
                item.Position = Position + new Vector2(0, heightOffset);
                heightOffset += item.Height;
            }
        }
    }
}
