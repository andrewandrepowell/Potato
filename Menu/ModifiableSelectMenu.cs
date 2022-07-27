using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class ModifiableSelectMenu : IMenu, ISelectable
    {
        private static SpriteFont font;
        private static readonly Color textColor = Potato.ColorTheme0;
        private readonly ControllerAlphaChanger controllerAlphaChanger;
        private readonly SelectChanger selectChanger;
        private readonly VisibilityStateChanger visibilityStateChanger = new VisibilityStateChanger();
        private Size2 size;
        private string currentText;
        private Vector2 textOffset;
        private Texture2D glowTexture;
        private Vector2 glowOffset;
        private Alignment align;
        public bool Selected { get => selectChanger.Selected; }
        public IController Controller { get; set; }
        public Vector2 Position { get; set; }
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get => visibilityStateChanger.State; }
        public string Text 
        {
            get => currentText;
            set
            {
                Debug.Assert(font.MeasureString(value).X <= size.Width);
                if (currentText != value)
                {
                    currentText = value;
                    float widthOffset = 0;
                    switch (align)
                    {
                        case Alignment.Left:
                            widthOffset = 0;
                            break;
                        case Alignment.Center:
                            widthOffset = (size.Width - font.MeasureString(currentText).X) / 2;
                            break;
                        case Alignment.Right:
                            widthOffset = size.Width - font.MeasureString(currentText).X;
                            break;
                    }
                    Vector2 textSize = font.MeasureString(currentText);
                    textOffset = new Vector2(
                        x: widthOffset,
                        y: (size.Height - textSize.Y) / 2);
                    glowTexture = font.CreateStandardGlow(currentText);
                    glowOffset = new Vector2(
                        x: textOffset.X - (glowTexture.Width - textSize.X) / 2,
                        y: textOffset.Y - (glowTexture.Height - textSize.Y) / 2);
                }
            }
        }

        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);

        public ModifiableSelectMenu(Alignment align, float width)
        {
            Debug.Assert(width > 0);
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
            size = new Size2(
                width: width,
                height: font.MeasureString(" ").Y + 8);
            this.align = align;
            controllerAlphaChanger = new ControllerAlphaChanger(controllable: this);
            selectChanger = new SelectChanger();
            Text = "";
            Controller = null;
            Position = Vector2.Zero;
        }

        public void OpenMenu() => visibilityStateChanger.OpenMenu();

        public void CloseMenu() => visibilityStateChanger.CloseMenu();

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexture,
                position: Position + glowOffset,
                color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
            spriteBatch.DrawString(
                spriteFont: font,
                text: Text,
                position: Position + textOffset,
                color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * selectChanger.ApplySelect(textColor: textColor));
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // The following are operations if the controller is set.
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
