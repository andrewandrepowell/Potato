using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Potato.Menu
{
    internal class SelectMenu : IMenu
    {
        private static SpriteFont font;
        private static readonly Color textColor = Potato.ColorTheme0;
        private readonly List<(string, Vector2, Texture2D, Vector2)> items = new List<(string, Vector2, Texture2D, Vector2)>();
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement = false;
        private float alpha = 1.0f;
        private static readonly Color selectColor = Potato.ColorTheme1;
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement = true;
        private float selectValue = 0.0f;
        public bool Selected { get; private set; } = false;
        public string Text { get; set; } = "";
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get; set; } = Size2.Empty;
        public Alignment Align { get; set; } = Alignment.Left;
        
        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);
        
        public void ApplyChanges()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            items.Clear();
            int heightIndex = 0;
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).X > Size.Width)
                {
                    Debug.Assert(currentLine != "", $"Width {Size.Width} not large enough for token {token.Value}.");
                    string newLine = currentLine.Trim();
                    float widthOffset = 0;
                    switch (Align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (Size.Width - font.MeasureString(newLine).X) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = Size.Width - font.MeasureString(newLine).X;
                            break;
                    }
                    float heightOffset = heightIndex * font.MeasureString(" ").Y;
                    Vector2 newLineOffset = new Vector2(x: widthOffset, y: heightOffset);
                    Texture2D glowTexture = font.CreateStandardGlow(newLine);
                    Vector2 newLineSize = font.MeasureString(newLine);
                    Vector2 glowOffset = new Vector2(
                        x: newLineOffset.X - (glowTexture.Width - newLineSize.X) / 2,
                        y: newLineOffset.Y - (glowTexture.Height - newLineSize.Y) / 2);
                    items.Add((newLine, newLineOffset, glowTexture, glowOffset));
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
                            widthOffset = (Size.Width - font.MeasureString(newLine).X) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = Size.Width - font.MeasureString(newLine).X;
                            break;
                    }
                    float heightOffset = heightIndex * font.MeasureString(" ").Y;
                    Vector2 newLineOffset = new Vector2(x: widthOffset, y: heightOffset);
                    Texture2D glowTexture = font.CreateStandardGlow(newLine);
                    Vector2 newLineSize = font.MeasureString(newLine);
                    Vector2 glowOffset = new Vector2(
                        x: newLineOffset.X - (glowTexture.Width - newLineSize.X) / 2,
                        y: newLineOffset.Y - (glowTexture.Height - newLineSize.Y) / 2);
                    items.Add((newLine, newLineOffset, glowTexture, glowOffset));
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: items.Count * font.MeasureString(" ").Y);
        }
        
        public SelectMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
        }

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix);
            foreach ((string newLine, Vector2 newLineOffset, Texture2D glowTexture, Vector2 glowOffset) in items)
            {
                spriteBatch.Draw(
                    texture: glowTexture,
                    position: Position + glowOffset,
                    color: alpha * Color.White);
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: newLine,
                    position: Position + newLineOffset,
                    color: alpha * (Add((1.0f - selectValue) * textColor, selectValue * selectColor)));
            }
            spriteBatch.End();
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
