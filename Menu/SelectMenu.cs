using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class SelectMenu : IMenu
    {
        private static BitmapFont font;
        private static readonly Color textColor = Color.Black;
        private readonly List<string> lines;
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement;
        private float alpha;
        private static readonly Color selectColor = Color.White;
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement;
        private float selectValue;
        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);
        public bool Selected { get; private set; }
        public void ApplyChanges()
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
        public SelectMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
            Controller = null;
            alpha = 1.0f;
            alphaIncrement = false;
            Selected = false;
            selectValueIncrement = true;
            selectValue = 0.0f;
        }
        public string Text { get; set; }
        public IController Controller { get; set; }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(
                    font: font,
                    text: line,
                    position: Position + new Vector2(0, index * font.LineHeight),
                    color: alpha * (Add((1.0f - selectValue) * textColor, selectValue * selectColor)));
        }
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Controller != null)
            {
                if (Controller.ActivatePressed())
                {
                    Selected = !Selected;
                }
            }

            if (Controller != null)
            {
                alpha += (alphaIncrement ? 1.0f : -1.0f) * alphaChangeRate * timeElapsed;
                if (alpha > 1.0f)
                    alphaIncrement = false;
                else if (alpha < 0.0f)
                    alphaIncrement = true;
            }
            else
            {
                alpha = 1.0f;
                alphaIncrement = false;
            }

            if (Selected)
            {
                selectValue += (selectValueIncrement ? 1.0f : -1.0f) * selectValueChangeRate * timeElapsed;
                if (selectValue > 1.0f)
                    selectValueIncrement = false;
                else if (selectValue < 0.0f)
                    selectValueIncrement = true;
            }
            else
            {
                selectValue = 0.0f;
                selectValueIncrement = true;
            }
        }
    }
}
