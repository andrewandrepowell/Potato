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
        private ControllerAlphaChanger controllerAlphaChanger;
        private static readonly Color selectColor = Potato.ColorTheme1;
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement = true;
        private float selectValue = 0.0f;
        private Size2 size;
        private VisibilityStateChanger visibilityStateChanger = new VisibilityStateChanger();
        private string currentText;
        private bool updateText;
        private Vector2 textOffset;
        private Texture2D glowTexture;
        private Vector2 glowOffset;
        private Alignment align;
        public bool Selected { get; set; } = false;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
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
                    updateText = true;
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
            currentText = null;
            updateText = false;
            this.align = align;
            controllerAlphaChanger = new ControllerAlphaChanger(controllable: this);
        }

        public void OpenMenu() => visibilityStateChanger.OpenMenu();

        public void CloseMenu() => visibilityStateChanger.CloseMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            if (updateText)
            {
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
                textOffset = new Vector2(
                    x: widthOffset,
                    y: (size.Height - font.MeasureString(currentText).Y) / 2);
                glowTexture = font.CreateStandardGlow(currentText);
                glowOffset = new Vector2(
                    x: widthOffset + glowTexture.Width / 2,
                    y: (size.Height - glowTexture.Height) / 2);
                updateText = false;
            }
            if (currentText != null)
            {
                spriteBatch.Begin(transformMatrix: transformMatrix);
                spriteBatch.Draw(
                    texture: glowTexture,
                    position: Position + glowOffset,
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: Text,
                    position: Position + textOffset,
                    color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * (Add((1.0f - selectValue) * textColor, selectValue * selectColor)));
                spriteBatch.End();
            }
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // The following are operations if the controller is set.
            if (Controller != null)
            {
                // If the controller activate is pressed, toggle select.
                if (Controller.ActivatePressed())
                {
                    Selected = !Selected;
                }
            }

            // If selected, flash with select color.
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

            visibilityStateChanger.Update(gameTime);
            controllerAlphaChanger.Update(gameTime);
        }
    }
}
