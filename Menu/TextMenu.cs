using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Potato.Menu
{
    internal class TextMenu : IMenu
    {
        private static SpriteFont font;
        private static readonly Color color = Potato.ColorTheme0;
        private readonly List<(string, Vector2, Texture2D, Vector2)> items;
        private readonly VisibilityStateChanger visibilityStateChanger;
        private Size2 size;
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; }
        public Size2 Size { get => size; set => throw new NotImplementedException(); }
        public MenuState State { get => visibilityStateChanger.State; }

        public TextMenu(string text, Alignment align, float width)
        {
            Debug.Assert(width > 0);

            // Initialize font.
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");

            // Initialize items. The text is split into lines depending on width.
            // Text textures, glow textures, and their respectives offsets are also created and stored.
            items = new List<(string, Vector2, Texture2D, Vector2)>();
            int heightIndex = 0;
            string currentLine = "";
            foreach (var token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).X > width)
                {
                    Debug.Assert(currentLine != "", $"Width {width} not large enough for token {token.Value}.");
                    string newLine = currentLine.Trim();
                    float widthOffset = 0;
                    switch (align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (width - font.MeasureString(newLine).X) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = width - font.MeasureString(newLine).X;
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
                    switch (align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (width - font.MeasureString(newLine).X) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = width - font.MeasureString(newLine).X;
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
            size = new Size2(
                width: width,
                height: items.Count * font.MeasureString(" ").Y + 8);
            visibilityStateChanger = new VisibilityStateChanger();
            Position = Vector2.Zero;
        }

        public void OpenMenu() => visibilityStateChanger.OpenMenu();

        public void CloseMenu() => visibilityStateChanger.CloseMenu();

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            foreach ((string newLine, Vector2 newLineOffset, Texture2D glowTexture, Vector2 glowOffset) in items)
            {
                spriteBatch.Draw(
                    texture: glowTexture, 
                    position: Position + glowOffset, 
                    color: visibilityStateChanger.Alpha * Color.White);
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: newLine,
                    position: Position + newLineOffset,
                    color: visibilityStateChanger.Alpha * color);
            }
            spriteBatch.End();
        }
        
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update state.
            visibilityStateChanger.Update(gameTime);
        }
    }
}
