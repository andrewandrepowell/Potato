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
        private List<(Texture2D, Texture2D, Vector2)> items;
        private static readonly Color fillColor = Potato.ColorTheme0;
        private const float fillChangeRate = 0.1f;
        private readonly ControllerAlphaChanger controllerAlphaChanger;
        private readonly VisibilityStateChanger visibilityStateChanger;
        private Texture2D texture;
        private Texture2D glowTexure;
        private Vector2 glowOffset;
        private float currentFill;
        private const float height = 16;
        private Size2 size;
        public float Fill 
        { 
            get => currentFill;
            set
            {
                currentFill = value;
                (texture, glowTexure, glowOffset) = items[(int)MathHelper.Clamp(value: currentFill * resolution, min: 0, max: resolution - 1)];
            }
        }
        public IController Controller { get => controllerAlphaChanger.Controller; set => controllerAlphaChanger.Controller = value; }
        public Vector2 Position { get; set; }
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public OpenCloseState MenuState { get => visibilityStateChanger.State; }

        public SliderMenu(float width, float fill)
        {
            Debug.Assert(width > 0);
            size = new Size2(width: width, height: height + 8);
            controllerAlphaChanger = new ControllerAlphaChanger();
            visibilityStateChanger = new VisibilityStateChanger();

            items = new List<(Texture2D, Texture2D, Vector2)>(capacity: resolution);
            for (int i = 0; i < resolution; i++)
            {
                Texture2D texture = Potato.SpriteBatch.GetStandardCurvedRectangle0(
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

            Fill = fill;
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
                texture: glowTexure,
                position: Position + glowOffset,
                color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
            spriteBatch.Draw(
                texture: texture,
                position: Position,
                color: visibilityStateChanger.Alpha * controllerAlphaChanger.Alpha * Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
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

            visibilityStateChanger.Update(gameTime);
            controllerAlphaChanger.Update(gameTime);
        }

        public void SoftReset()
        {
            visibilityStateChanger.SoftReset();
            controllerAlphaChanger.SoftReset();
        }

        public void HardReset()
        {
            visibilityStateChanger.HardReset();
            controllerAlphaChanger.HardReset();
        }
    }
}
