using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Potato.Menu
{
    internal class ContainerMenu : IMenu
    {
        private List<(IMenu, Vector2)> items = new List<(IMenu, Vector2)>();
        private IController controller = null;
        private Texture2D backplaneTexture = null;
        private static readonly Color backPlaneColor0 = Potato.ColorTheme2;
        private static readonly Color backPlaneColor1 = Potato.ColorTheme3;
        private const float backPlaneEdgeRadius = 8;
        private Vector2 backPlaneOffset = Vector2.Zero;
        private Texture2D glowTexure = null;
        private Vector2 glowOffset = Vector2.Zero;
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

        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            // Generate the backplane.
            if (applyChanges || backplaneTexture == null)
            {
                Size2 size = Size;
                Size fullSize = new Size(
                    width: (int)Math.Ceiling(size.Width + backPlaneEdgeRadius * 2),
                    height: (int)Math.Ceiling(size.Height + backPlaneEdgeRadius * 2));
                float diagonalLength = (float)Math.Sqrt(fullSize.Width * fullSize.Width + fullSize.Height * fullSize.Height);
                Color GetColor(Point point)
                {
                    float distanceFromTopLeft = Vector2.Distance(
                        point.ToVector2(), 
                        new Vector2(x: fullSize.Width, y: fullSize.Height));
                    float fadeRatio = distanceFromTopLeft / diagonalLength;
                    return Add(
                        color1: fadeRatio * backPlaneColor0,
                        color2: (1 - fadeRatio) * backPlaneColor1);
                }
                backplaneTexture = spriteBatch.GetCurvedRectangle(
                    size: fullSize,
                    edgeRadius: backPlaneEdgeRadius,
                    color: GetColor);
                backPlaneOffset = new Vector2(x: -backPlaneEdgeRadius, y: -backPlaneEdgeRadius);
                glowTexure = backplaneTexture.CreateStandardGlow0();
                glowOffset = new Vector2(
                    x: backPlaneOffset.X - (glowTexure.Width - backplaneTexture.Width) / 2,
                    y: backPlaneOffset.Y - (glowTexure.Height - backplaneTexture.Height) / 2);
            }
            applyChanges = false;

            // Draw the backplane.
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexure,
                position: Position + glowOffset,
                color: Color.White);
            spriteBatch.Draw(
                texture: backplaneTexture, 
                position: Position + backPlaneOffset, 
                color: Color.White);
            spriteBatch.End();

            // Draw the other elements of the menu.
            foreach ((IMenu item, _) in items)
                item.Draw(spriteBatch: spriteBatch, transformMatrix: transformMatrix);
        }
        
        public void Update(GameTime gameTime)
        {
            foreach ((IMenu item, Vector2 itemOffset) in items)
            {
                item.Position = Position + itemOffset;
                item.Update(gameTime);
            }

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
            items.Clear();
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
                Vector2 itemOffset = new Vector2(widthOffset, heightOffset);
                items.Add((item, itemOffset));
                heightOffset += item.Size.Height;
            }

            // Set the apply flag so that changes that can only be in drawing can occur.
            applyChanges = true;
        }
    }
}
