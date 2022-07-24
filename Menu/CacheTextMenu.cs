using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class CacheTextMenu : IMenu, ISelectable
    {
        private static SpriteFont font;
        private static readonly Color textColor = Potato.ColorTheme0;
        private Dictionary<string, List<(string, Texture2D, Vector2, Vector2)>> items;
        private string currentText;
        private List<(string, Texture2D, Vector2, Vector2)> currentLines;
        private VisibilityStateChanger visibilityStateChanger;
        private ControllerAlphaChanger controllerAlphaChanger;
        private SelectChanger selectChanger;
        private Size2 menuSize;
        public string Text
        {
            get => currentText;
            set
            {
                if (value == currentText)
                    return;
                currentText = value;
                currentLines = items[currentText];
            }
        }
        public MenuState State => visibilityStateChanger.State;
        public IController Controller { get; set; }
        public Vector2 Position { get; set; }
        public Size2 Size { get => menuSize; set { } }
        public bool Selected { get; set; }

        public CacheTextMenu(IList<string> texts, Alignment align, float width)
        {
            Debug.Assert(texts.Count > 0);
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
            
            items = new Dictionary<string, List<(string, Texture2D, Vector2, Vector2)>>();
            foreach (string text in texts)
            {
                var lines = new List<(string, Texture2D, Vector2, Vector2)>();
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
                        lines.Add((newLine, glowTexture, newLineOffset, glowOffset));
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
                        lines.Add((newLine, glowTexture, newLineOffset, glowOffset));
                    }
                    else
                    {
                        currentLine += token.Value + " ";
                    }
                }
                items.Add(text, lines);
            }
            
            Text = texts[0];
            visibilityStateChanger = new VisibilityStateChanger();
            controllerAlphaChanger = new ControllerAlphaChanger(controllable: this);
            selectChanger = new SelectChanger(selectable: this);
            Controller = null;
            Position = Vector2.Zero;
            menuSize = new Size2(
                width: width,
                height: items.Select((tuple) => tuple.Value.Sum((lines) => font.MeasureString(lines.Item1).Y)).Max());
        }

        public void CloseMenu() => visibilityStateChanger.CloseMenu();

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            foreach (var (text, glowTexture, textOffset, glowOffset) in currentLines)
            {
                spriteBatch.Draw(
                    texture: glowTexture, 
                    position: Position + glowOffset, 
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
                spriteBatch.DrawString(
                    spriteFont: font, 
                    text: text,
                    position: Position + textOffset, 
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * selectChanger.ApplySelect(textColor: textColor));
            }
            spriteBatch.End();
        }

        public void OpenMenu() => visibilityStateChanger.OpenMenu();

        public void Update(GameTime gameTime)
        {
            if (Controller != null && Controller.ActivatePressed())
                Selected = !Selected;

            visibilityStateChanger.Update(gameTime: gameTime);
            controllerAlphaChanger.Update(gameTime: gameTime);
            selectChanger.Update(gameTime: gameTime);
        }
    }
}
