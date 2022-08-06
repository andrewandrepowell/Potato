using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Potato.Room
{
    internal class RoomStateChanger : IRoom
    {
        private static Color fadeColor;
        private static Texture2D fadeTexture;
        private const float fadeAlphaChangeRate = 3.0f;
        private float fadeAlpha;
        public OpenCloseState RoomState { get; private set; }
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
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
            fadeAlpha = 1.0f;
            RoomState = OpenCloseState.Closed;
        }

        public void CloseRoom() => RoomState = OpenCloseState.Closing;

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(texture: fadeTexture, position: Vector2.Zero, color: Color.White * fadeAlpha);
            spriteBatch.End();
        }

        public void OpenRoom() => RoomState = OpenCloseState.Opening;

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (RoomState)
            {
                case OpenCloseState.Closing:
                    fadeAlpha = Math.Min(fadeAlpha + timeElapsed * fadeAlphaChangeRate, 1.0f);
                    if (fadeAlpha == 1.0f)
                        RoomState = OpenCloseState.Closed;
                    break;
                case OpenCloseState.Opening:
                    fadeAlpha = Math.Max(fadeAlpha - timeElapsed * fadeAlphaChangeRate, 0.0f);
                    if (fadeAlpha == 0.0f)
                        RoomState = OpenCloseState.Opened;
                    break;
            }
        }
    }
}
