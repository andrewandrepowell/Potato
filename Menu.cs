using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Potato
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
        public void Apply()
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
                    .Select((x) => (((float)(x % texture.Width)/texture.Width) > Fill) ? emptyColor : fillColor)
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
                alpha += ((alphaIncrement) ? 1.0f : -1.0f) * alphaChangeRate * timeElapsed;
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
        }
    }
    internal class SelectMenu : IMenu
    {
        private static BitmapFont font;
        private static readonly Color color = Color.Black;
        private readonly List<string> lines;
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement;
        private float alpha;
        public void Apply()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Size.Width)
                {
                    lines.Add(currentLine.Trim());
                    currentLine = token.Value + " ";
                }
                else if (token.IsLast)
                {
                    currentLine += token.Value;
                    lines.Add(currentLine.Trim());
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: lines.Count * font.LineHeight);
        }
        public SelectMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
            Controller = null;
            alpha = 1.0f;
            alphaIncrement = false;
        }
        public string Text { get; set; }
        public IController Controller { get; set; }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(
                    font: font,
                    text: line,
                    position: Position + new Vector2(0, index * font.LineHeight),
                    color: alpha * color);
        }
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Controller != null)
            {
                alpha += ((alphaIncrement) ? 1.0f : -1.0f) * alphaChangeRate * timeElapsed;
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
        }
    }
    internal class DividerMenu : IMenu
    {
        private static Texture2D texture;
        private static readonly Color color = Color.Black;
        public DividerMenu()
        {
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public void Apply()
        {
        }
        public void Draw(SpriteBatch spriteBatch)
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
            DrawLine(
                spriteBatch: spriteBatch,
                point1: Position,
                point2: Position + new Vector2(x: Size.Width, y: 0),
                color: color,
                thickness: Size.Height);
        }
        public void Update(GameTime gameTime)
        {
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
    internal class TextMenu : IMenu
    {
        private static BitmapFont font;
        private static readonly Color color = Color.Black;
        private readonly List<string> lines;
        public void Apply()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            lines.Clear();
            string currentLine = "";
            foreach (var token in Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Detailed())
            {
                if (font.MeasureString(currentLine + token.Value).Width > Size.Width)
                {
                    lines.Add(currentLine.Trim());
                    currentLine = token.Value + " ";
                }
                else if (token.IsLast)
                {
                    currentLine += token.Value;
                    lines.Add(currentLine.Trim());
                }
                else
                {
                    currentLine += token.Value + " ";
                }
            }
            Size = new Size2(
                width: Size.Width, 
                height: lines.Count * font.LineHeight);
        }
        public TextMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            lines = new List<string>();
            Text = "";
            Position = Vector2.Zero;
            Size = Size2.Empty;
        }
        public string Text { get; set; }
        public IController Controller { get => null; set { } }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((int index, string line) in lines.Select((line, index) => (index, line)))
                spriteBatch.DrawString(
                    font: font, 
                    text: line, 
                    position: Position + new Vector2(0, index * font.LineHeight), 
                    color: color);
        }
        public void Update(GameTime gameTime)
        {
        }
    }
    internal class Menu : IMenu
    {
        public Menu()
        {
            Items = new List<IMenu>();
            Position = Vector2.Zero;
        }
        public List<IMenu> Items { get; private set; }
        public Vector2 Position { get; set; }
        public Size2 Size 
        { 
            get => new Size2(
                width: Items.Select((item) => item.Size.Width).Max(), 
                height: Items.Select((item) => item.Size.Height).Sum());
            set { }
        }
        public IController Controller { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IMenu item in Items)
                item.Draw(spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
            foreach (IMenu item in Items)
                item.Update(gameTime);

            if (Controller != null)
            {
                int numberOfItemsWithAController = Items.Where((x) => x.Controller != null).Count();

                if (numberOfItemsWithAController > 1)
                    throw new Exception();
                
                //if ()
            }
        }
        public void Apply()
        {
            float heightOffset = 0;
            foreach (IMenu item in Items)
            {
                item.Apply();
                item.Position = Position + new Vector2(0, heightOffset);
                heightOffset += item.Size.Height;
            }
        }
    }
}
