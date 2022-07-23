using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.GlowEffect;
using MonoGame.Extended;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class SliderMenu : IMenu
    {
        private const int resolution = 64;
        private List<(Texture2D, Texture2D, Vector2)> items = null;
        private static readonly Color fillColor = Potato.ColorTheme0;
        private ControllerAlphaChanger controllerAlphaChanger;
        private const float fillChangeRate = 0.1f;
        private Texture2D texture = null;
        private Texture2D glowTexure = null;
        private Vector2 glowOffset = Vector2.Zero;
        private float currentFill = 0.0f;
        private const float height = 16;
        private float width;
        private Size2 size;
        private bool initialize = true;
        private VisibilityStateChanger state = new VisibilityStateChanger();
        public float Fill { get; set; } = 0.0f;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get => state.State; }

        public SliderMenu(float width, float fill)
        {
            Debug.Assert(width > 0);
            this.width = width;
            size = new Size2(width: width, height: height + 8);
            Fill = fill;
            controllerAlphaChanger = new ControllerAlphaChanger(controllable: this);
        }

        public void OpenMenu() => state.OpenMenu();

        public void CloseMenu() => state.CloseMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            // Initialize all the possible textures with their respective glow textures. 
            if (items == null)
            {
                items = new List<(Texture2D, Texture2D, Vector2)>(capacity: resolution);
                for (int i = 0; i < resolution; i++)
                {
                    Texture2D texture = spriteBatch.GetStandardCurvedRectangle0(
                        size: new Size(
                            width: Math.Max((int)MathHelper.Lerp(0, width, (float)(i) / resolution), 1),
                            height: (int)height),
                        color: (_) => fillColor);
                    Texture2D glowTexure = texture.CreateStandardGlow0();
                    Vector2 glowOffset = new Vector2(
                        x: -(glowTexure.Width - texture.Width) / 2,
                        y: -(glowTexure.Height - texture.Height) / 2);
                    items.Add((texture, glowTexure, glowOffset));
                }
            }

            // Update the selected texture if a need Fill is specified.
            if (currentFill != Fill || initialize)
            {
                initialize = false;
                currentFill = Fill;
                (texture, glowTexure, glowOffset) = items[(int)MathHelper.Clamp(value: currentFill * resolution, min: 0, max: resolution - 1)];
            }

            // Draw the currently selected texture.
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexure,
                position: Position + glowOffset,
                color: state.Alpha * controllerAlphaChanger.Alpha * Color.White);
            spriteBatch.Draw(
                texture: texture,
                position: Position,
                color: state.Alpha * controllerAlphaChanger.Alpha * Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Following are operations if a controller is set.
            if (Controller != null)
            {
                // Controller can be used to change the fill.
                if (Controller.LeftPressed())
                {
                    Fill -= fillChangeRate;
                    if (Fill < 0.0f)
                        Fill = 0.0f;
                }
                if (Controller.RightPressed())
                {
                    Fill += fillChangeRate;
                    if (Fill > 1.0f)
                        Fill = 1.0f;
                }
            }

            state.Update(gameTime);
            controllerAlphaChanger.Update(gameTime);
        }
    }
}
