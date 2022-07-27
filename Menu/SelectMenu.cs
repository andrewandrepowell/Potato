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
    internal class SelectMenu : IMenu, ISelectable
    {
        private static SpriteFont font;
        private static readonly Color textColor = Potato.ColorTheme0;
        private readonly List<(string, Vector2, Texture2D, Vector2)> items;
        private ControllerAlphaChanger controllerAlphaChanger;
        private VisibilityStateChanger visibilityStateChanger;
        private SelectChanger selectChanger;
        private Size2 size;

        public bool Selected { get => selectChanger.Selected; }
        public IController Controller { get => controllerAlphaChanger.Controller; set => controllerAlphaChanger.Controller = value; }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get => visibilityStateChanger.State; }

        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);

        public SelectMenu(string text, Alignment align, float width)
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
                    heightIndex++;
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }

            
            size = new Size2(
                width: width,
                height: items.Count * font.MeasureString(" ").Y + 8);
            controllerAlphaChanger = new ControllerAlphaChanger();
            visibilityStateChanger = new VisibilityStateChanger();
            selectChanger = new SelectChanger();
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
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: newLine,
                    position: Position + newLineOffset,
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * selectChanger.ApplySelect(textColor: textColor));
            }
            spriteBatch.End();
        }
        
        public void Update(GameTime gameTime)
        {
            if (Controller != null && Controller.ActivatePressed())
                selectChanger.Select();

            visibilityStateChanger.Update(gameTime);
            controllerAlphaChanger.Update(gameTime);
            selectChanger.Update(gameTime);
        }

        public void Select() => selectChanger.Select();

        public void ResetMedia() => selectChanger.ResetMedia();
    }
}
