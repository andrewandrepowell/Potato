using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.GlowEffect;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class SliderMenu : IMenu
    {
        private const int resolution = 64;
        private static List<(Texture2D, Texture2D, Vector2)> textures = null;
        private static readonly Color fillColor = Potato.ColorTheme0;
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement = false;
        private float alpha = 1.0f;
        private bool apply = false;
        private const float fillChangeRate = 0.1f;
        private Texture2D texture = null;
        private Texture2D glowTexure = null;
        private Vector2 glowOffset = Vector2.Zero;
        public float Fill { get; set; } = 0.0f;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get; set; } = Size2.Empty;
        public Alignment Align { get; set; } = Alignment.Left;
        

        public void ApplyChanges()
        {
            apply = true;
        }
        
        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            if (textures == null)
            {
                textures = new List<(Texture2D, Texture2D, Vector2)>(capacity: resolution);
                for (int i = 0; i < resolution; i++)
                {
                    Texture2D texture = spriteBatch.GetStandardCurvedRectangle0(
                        size: new Size(
                            width: Math.Max((int)MathHelper.Lerp(0, Size.Width, (float)(i + 1) / resolution), 1),
                            height: (int)Size.Height),
                        color: (_) => fillColor);
                    Texture2D glowTexure = texture.CreateStandardGlow0();
                    Vector2 glowOffset = new Vector2(
                        x: -(glowTexure.Width - texture.Width) / 2,
                        y: -(glowTexure.Height - texture.Height) / 2);
                    textures.Add((texture, glowTexure, glowOffset));
                }
            }
            if (texture == null || apply)
            {
                (texture, glowTexure, glowOffset) = textures[(int)MathHelper.Clamp(value: Fill * resolution, min: 0, max: resolution - 1)];
            }
            apply = false;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexure,
                position: Position + glowOffset,
                color: alpha * Color.White);
            spriteBatch.Draw(
                texture: texture,
                position: Position,
                color: alpha * Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            if (Controller != null)
            {
                if (Controller.LeftPressed())
                {
                    Fill -= fillChangeRate;
                    if (Fill < 0.0f)
                        Fill = 0.0f;
                    ApplyChanges();
                }
                if (Controller.RightPressed())
                {
                    Fill += fillChangeRate;
                    if (Fill > 1.0f)
                        Fill = 1.0f;
                    ApplyChanges();
                }
            }
        }
    }
}
