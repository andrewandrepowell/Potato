using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;

namespace Potato.Room
{
    internal class RoomStateChanger : IRoom, IMovable
    {
        private static Color fadeColor;
        private static Texture2D fadeTexture;
        private const float fadeAlphaChangeRate = 3.0f;
        private float fadeAlpha;
        private Vector2 position;
        public IOpenable.OpenStates OpenState { get; private set; }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Position { get => position; set => position = value; }

        static RoomStateChanger()
        {
            fadeColor = Potato.ColorTheme3;
            fadeTexture = null;
        }
        
        public RoomStateChanger()
        {
            if (fadeTexture == null)
            {
                int width = Potato.Game.GraphicsDevice.Viewport.Width;
                int height = Potato.Game.GraphicsDevice.Viewport.Height;
                fadeTexture = new Texture2D(
                    graphicsDevice: Potato.Game.GraphicsDevice,
                    width: width,
                    height: height,
                    mipmap: false,
                    format: SurfaceFormat.Color);
                fadeTexture.SetData(Enumerable.Range(0, width * height).Select(i => fadeColor).ToArray());
            }
            HardReset();
        }
        
        public void Close() => OpenState = IOpenable.OpenStates.Closing;

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: fadeTexture, position: position, color: Color.White * fadeAlpha);
            spriteBatch.End();
        }

        public void Open() => OpenState = IOpenable.OpenStates.Opening;

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (OpenState)
            {
                case IOpenable.OpenStates.Closing:
                    fadeAlpha = Math.Min(fadeAlpha + timeElapsed * fadeAlphaChangeRate, 1.0f);
                    if (fadeAlpha == 1.0f)
                        OpenState = IOpenable.OpenStates.Closed;
                    break;
                case IOpenable.OpenStates.Opening:
                    fadeAlpha = Math.Max(fadeAlpha - timeElapsed * fadeAlphaChangeRate, 0.0f);
                    if (fadeAlpha == 0.0f)
                        OpenState = IOpenable.OpenStates.Opened;
                    break;
            }
        }

        public void SoftReset()
        {
            fadeAlpha = 1.0f;
            OpenState = IOpenable.OpenStates.Closed;
        }

        public void HardReset()
        {
            position = Vector2.Zero;
            SoftReset();
        }
    }
}
