using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Potato.Menu
{
    internal class ContainerMenu : IMenu
    {
        private IController controller = null;
        private Texture2D backplaneTexture = null;
        private static readonly Color backPlaneColor = Potato.ColorTheme2;
        private const float backPlaneEdgeRadius = 16;
        private Vector2 backPlaneOffset = Vector2.Zero;
        private bool applyChanges = false;
        public Alignment Align { get; set; } = Alignment.Left;
        public List<IMenu> Items { get; private set; } = new List<IMenu>();
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size
        {
            get => new Size2(
                width: Items.Select((item) => item.Size.Width).Max(),
                height: Items.Select((item) => item.Size.Height).Sum());
            set { }
        }
        public IController Controller
        {
            get => controller;
            set
            {
                bool controllerSet = false;
                foreach (IMenu item in Items)
                {
                    if (controllerSet)
                    {
                        item.Controller = null;
                    }
                    else
                    {
                        item.Controller = value;
                        if (item.Controller == value)
                            controllerSet = true;
                    }
                }
                if (controllerSet)
                    controller = value;
                else
                    controller = null;
            }
        }
        
        private void DrawBackplane(SpriteBatch spriteBatch)
        {
            
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            // Generate the backplane.
            if (applyChanges || backplaneTexture == null)
            {
                Size2 size = Size;
                int width = (int)Math.Ceiling(size.Width + 2 * backPlaneEdgeRadius);
                int height = (int)Math.Ceiling(size.Height + 2 * backPlaneEdgeRadius);
                backplaneTexture = new Texture2D(
                    graphicsDevice: spriteBatch.GraphicsDevice,
                    width: width,
                    height: height,
                    mipmap: false,
                    format: SurfaceFormat.Color);
                Color[] colors = new Color[width * height];
                RectangleF bounds = new RectangleF(x: backPlaneEdgeRadius, y: backPlaneEdgeRadius, width: size.Width, height: size.Height);
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        Point2 point = new Point2(x, y);
                        if (point.X < bounds.TopLeft.X && point.Y < bounds.TopLeft.Y && Vector2.Distance(bounds.TopLeft, point) > backPlaneEdgeRadius)
                            colors[y * width + x] = Color.Transparent;
                        else if (point.X > bounds.TopRight.X && point.Y < bounds.TopRight.Y && Vector2.Distance(bounds.TopRight, point) > backPlaneEdgeRadius)
                            colors[y * width + x] = Color.Transparent;
                        else if (point.X > bounds.BottomRight.X && point.Y > bounds.BottomRight.Y && Vector2.Distance(bounds.BottomRight, point) > backPlaneEdgeRadius)
                            colors[y * width + x] = Color.Transparent;
                        else if (point.X < bounds.BottomLeft.X && point.Y > bounds.BottomLeft.Y && Vector2.Distance(bounds.BottomLeft, point) > backPlaneEdgeRadius)
                            colors[y * width + x] = Color.Transparent;
                        else
                            colors[y * width + x] = backPlaneColor;
                    }
                backplaneTexture.SetData(colors);
                backPlaneOffset = new Vector2(x: -backPlaneEdgeRadius, y: -backPlaneEdgeRadius);
            }
            applyChanges = false;

            // Draw the backplane.
            spriteBatch.Draw(
                texture: backplaneTexture, 
                position: Position + backPlaneOffset, 
                color: Color.White);

            // Draw the other elements of the menu.
            foreach (IMenu item in Items)
                item.Draw(spriteBatch);
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (IMenu item in Items)
                item.Update(gameTime);

            if (Controller != null)
            {
                if (Controller.DownPressed())
                {
                    int indexWithController = Items.FindIndex((item) => item.Controller == Controller);
                    if (indexWithController >= 0)
                    {
                        Items[indexWithController].Controller = null;
                        for (int index = 0; index < Items.Count; index++)
                        {
                            int nextIndex = (indexWithController + 1 + index) % Items.Count;
                            Items[nextIndex].Controller = Controller;
                            if (Items[nextIndex].Controller == Controller)
                                break;
                        }
                    }
                }
                if (Controller.UpPressed())
                {
                    int indexWithController = Items.FindIndex((item) => item.Controller == Controller);
                    if (indexWithController >= 0)
                    {
                        Items[indexWithController].Controller = null;
                        for (int index = 0; index < Items.Count; index++)
                        {
                            int prevIndex = ((indexWithController - (1 + index)) % Items.Count + Items.Count) % Items.Count;
                            Items[prevIndex].Controller = Controller;
                            if (Items[prevIndex].Controller == Controller)
                                break;
                        }
                    }
                }
            }
        }
        public void ApplyChanges()
        {
            // Apply changes to each of the menu items.
            Size2 size = Size;
            float heightOffset = 0;
            foreach (IMenu item in Items)
            {
                item.Align = Align;
                item.ApplyChanges();
                float widthOffset = 0;
                switch (Align)
                {
                    case Alignment.Center:
                        widthOffset = (size.Width - item.Size.Width) / 2;
                        break;
                    case Alignment.Right:
                        widthOffset = size.Width - item.Size.Width;
                        break;
                    case Alignment.Left:
                        widthOffset = 0;
                        break;
                }
                item.Position = Position + new Vector2(widthOffset, heightOffset);
                heightOffset += item.Size.Height;
            }

            // Set the apply flag so that changes that can only be in drawing can occur.
            applyChanges = true;
        }
    }
}
