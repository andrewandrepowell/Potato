using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class SelectImageMenu : IMenu
    {
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement = false;
        private float alpha = 1.0f;
        private static readonly Color selectColor = Potato.ColorTheme1;
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement = true;
        private float selectValue = 0.0f;
        private Texture2D texture;
        private Size2 size;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public bool Selected { get; set; } = false;

        public SelectImageMenu(Texture2D texture)
        {
            this.texture = texture;
            size = new Size2(texture.Width, texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: texture, 
                position: Position, 
                color: alpha * (Add((1.0f - selectValue) * Color.White, selectValue * selectColor)));
            spriteBatch.End();
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

                // Flash the textures.
                alpha += (alphaIncrement ? 1.0f : -1.0f) * alphaChangeRate * timeElapsed;
                if (alpha > 1.0f)
                    alphaIncrement = false;
                else if (alpha < 0.0f)
                    alphaIncrement = true;
            }
            else
            {
                // When there's no controller, don't flash.
                alpha = 1.0f;
                alphaIncrement = false;
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
        }

        private static Color Add(Color color1, Color color2) => new Color(
           color1.R + color2.R,
           color1.G + color2.G,
           color1.B + color2.B,
           color1.A + color2.A);
    }
}
