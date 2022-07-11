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
    internal interface IMenuItem
    {
        Vector2 Position { get; set; }
        float Width { get; set; }
        float Height { get; }
    }
    internal class TextMenuItem : IMenuItem, IDrawable
    {
        private static BitmapFont font;
        private string text;
        private float width;
        private List<string> lines;
        private void Update()
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            string currentLine = "";
            foreach (string token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (font.MeasureString(currentLine + token).Width > width)
                {
                    lines.Add(currentLine.Trim());
                    currentLine = token + " ";
                }
                else
                {
                    currentLine += token + " ";
                }
            }
        }
        public TextMenuItem()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Width = 0;
        }
        public string Text
        {
            get => text;
            set
            {
                text = value;
                Update();
            }
        }
        public float Width 
        {
            get => width;
            set
            {
                width = value;
                Update();
            }
        }
        public float Height => lines.Count * font.LineHeight;
        public Vector2 Position { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(font, line, Position + new Vector2(0, index * font.LineHeight), Color.White);
        }
    }
    internal class Menu : IComponent, IDestroyable
    {
        public Menu()
        {
            Destroyed = false;
            Activate = false;
            Items = new List<IMenuItem>();
            Controller = null;
        }
        public bool Destroyed { get; private set; }
        public bool Activate;
        public IController Controller;
        public List<IMenuItem> Items { get; private set; }
        public void Destroy()
        {
            Destroyed = true;
        }
        public void Close()
        {
            throw new NotImplementedException();
        }
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
    }
}
