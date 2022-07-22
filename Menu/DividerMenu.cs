using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;

namespace Potato.Menu
{
    internal class DividerMenu : IMenu
    {
        private static Texture2D texture;
        private static readonly Color color = Potato.ColorTheme0;
        private Size2 size;
        private float width, height;
        private VisibilityStateChanger state = new VisibilityStateChanger();
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get => state.State; }

        public DividerMenu(float width, float height)
        {
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);
            this.width = width;
            this.height = height;
            size = new Size2(width: width, height: height + 4);
        }

        public void OpenMenu() => state.OpenMenu();

        public void CloseMenu() => state.CloseMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            if (texture == null)
            {
                texture = new Texture2D(
                    graphicsDevice: spriteBatch.GraphicsDevice,
                    width: 1,
                    height: 1,
                    mipmap: false,
                    format: SurfaceFormat.Color);
                texture.SetData(new Color[] { Color.White });
            }
            spriteBatch.Begin(transformMatrix: transformMatrix);
            DrawLine(
                spriteBatch: spriteBatch,
                point1: Position,
                point2: Position + new Vector2(x: width, y: 0),
                color: state.Alpha * color,
                thickness: height);
            spriteBatch.End();
        }
        
        public void Update(GameTime gameTime)
        {
            state.Update(gameTime);
        }
        
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            Vector2 origin = new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2(length, thickness);
            spriteBatch.Draw(
                texture: texture,
                position: point,
                sourceRectangle: null,
                color: color,
                rotation: angle,
                origin: origin,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            float distance = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(
                spriteBatch: spriteBatch,
                point: point1,
                length: distance,
                angle: angle,
                color: color,
                thickness: thickness);
        }
    }
}
