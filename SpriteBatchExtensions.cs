using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal static class SpriteBatchExtensions
    {
        public delegate Color GetColorDelegate(Point point);
        public static Texture2D GetCurvedRectangle(this SpriteBatch spriteBatch, Size size, float edgeRadius, GetColorDelegate color)
        {
            Texture2D texture = new Texture2D(
                graphicsDevice: spriteBatch.GraphicsDevice,
                width: size.Width,
                height: size.Height,
                mipmap: false,
                format: SurfaceFormat.Color);
            Color[] colors = new Color[size.Width * size.Height];
            RectangleF innerBounds = new RectangleF(
                x: edgeRadius, 
                y: edgeRadius, 
                width: size.Width - 2 * edgeRadius, 
                height: size.Height - 2 * edgeRadius);
            for (int y = 0; y < size.Height; y++)
                for (int x = 0; x < size.Width; x++)
                {
                    Point2 point = new Point2(x, y);
                    if (point.X < innerBounds.TopLeft.X && point.Y < innerBounds.TopLeft.Y && Vector2.Distance(innerBounds.TopLeft, point) > edgeRadius)
                        colors[y * size.Width + x] = Color.Transparent;
                    else if (point.X > innerBounds.TopRight.X && point.Y < innerBounds.TopRight.Y && Vector2.Distance(innerBounds.TopRight, point) > edgeRadius)
                        colors[y * size.Width + x] = Color.Transparent;
                    else if (point.X > innerBounds.BottomRight.X && point.Y > innerBounds.BottomRight.Y && Vector2.Distance(innerBounds.BottomRight, point) > edgeRadius)
                        colors[y * size.Width + x] = Color.Transparent;
                    else if (point.X < innerBounds.BottomLeft.X && point.Y > innerBounds.BottomLeft.Y && Vector2.Distance(innerBounds.BottomLeft, point) > edgeRadius)
                        colors[y * size.Width + x] = Color.Transparent;
                    else
                        colors[y * size.Width + x] = color(new Point(x, y));
                }
            texture.SetData(colors);
            return texture;
        }
        public static Texture2D GetCurvedRectangle(this SpriteBatch spriteBatch, Size size, float edgeRadius, Color color) =>
            spriteBatch.GetCurvedRectangle(size: size, edgeRadius: edgeRadius, color: (point) => color);
    }
}
