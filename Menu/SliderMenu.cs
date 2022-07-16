﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class SliderMenu : IMenu
    {
        private static Texture2D texture;
        private static readonly Color fillColor = Color.Black;
        private static readonly Color emptyColor = Color.Transparent;
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement;
        private float alpha;
        private bool apply;
        private const float fillChangeRate = 0.1f;
        public SliderMenu()
        {
            apply = false;
            Fill = 0.0f;
            Controller = null;
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public float Fill { get; set; }
        public IController Controller { get; set; }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public Alignment Align { get; set; }

        public void ApplyChanges()
        {
            apply = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null || apply)
            {
                texture = new Texture2D(
                    graphicsDevice: spriteBatch.GraphicsDevice,
                    width: (int)Size.Width,
                    height: (int)Size.Height,
                    mipmap: false,
                    format: SurfaceFormat.Color);
                texture.SetData(Enumerable
                    .Range(0, texture.Width * texture.Height)
                    .Select((x) => (float)(x % texture.Width) / texture.Width > Fill ? emptyColor : fillColor)
                    .ToArray());
                apply = false;
            }
            spriteBatch.Draw(
                texture: texture,
                position: Position,
                color: alpha * fillColor);
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
