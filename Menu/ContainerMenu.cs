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
        private static readonly Color backPlaneColor0 = Potato.ColorTheme2;
        private static readonly Color backPlaneColor1 = Potato.ColorTheme3;
        private const float backPlaneEdgeRadius = 8;
        private List<(IMenu, Vector2)> items;
        private IController controller;
        private Texture2D backplaneTexture;
        private Vector2 backPlaneOffset;
        private Texture2D glowTexure;
        private Vector2 glowOffset;
        private Size2 size;
        private VisibilityStateChanger visibilityStateChanger;
        public Vector2 Position { get; set; }
        public Size2 Size { get => size; set => throw new NotImplementedException(); }
        public OpenCloseState MenuState { get; private set; } = OpenCloseState.Closed;
        public IController Controller
        {
            get => controller;
            set
            {
                bool controllerSet = false;
                foreach ((IMenu component, _) in items)
                {
                    if (controllerSet)
                    {
                        component.Controller = null;
                    }
                    else
                    {
                        component.Controller = value;
                        if (component.Controller == value)
                            controllerSet = true;
                    }
                }
                controller = value;
            }
        }

        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);

        public ContainerMenu(IList<IMenu> components, Alignment align)
        {
            size = new Size2(
                width: components.Select((component) => component.Size.Width).Max(),
                height: components.Select((component) => component.Size.Height).Sum());
            items = new List<(IMenu, Vector2)>(capacity: components.Count);
            float heightOffset = 0;
            foreach (IMenu component in components)
            {
                float widthOffset = 0;
                switch (align)
                {
                    case Alignment.Center:
                        widthOffset = (size.Width - component.Size.Width) / 2;
                        break;
                    case Alignment.Right:
                        widthOffset = size.Width - component.Size.Width;
                        break;
                    case Alignment.Left:
                        widthOffset = 0;
                        break;
                }
                Vector2 itemOffset = new Vector2(widthOffset, heightOffset);
                items.Add((component, itemOffset));
                heightOffset += component.Size.Height;
            }

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
            backplaneTexture = Potato.SpriteBatch.GetCurvedRectangle(
                size: fullSize,
                edgeRadius: backPlaneEdgeRadius,
                color: GetColor);
            backPlaneOffset = new Vector2(x: -backPlaneEdgeRadius, y: -backPlaneEdgeRadius);
            glowTexure = backplaneTexture.CreateStandardGlow0();
            glowOffset = new Vector2(
                x: backPlaneOffset.X - (glowTexure.Width - backplaneTexture.Width) / 2,
                y: backPlaneOffset.Y - (glowTexure.Height - backplaneTexture.Height) / 2);

            controller = null;
            Position = Vector2.Zero;
            MenuState = OpenCloseState.Closed;
            visibilityStateChanger = new VisibilityStateChanger();
        }

        public void OpenMenu()
        {
            MenuState = OpenCloseState.Opening;
            visibilityStateChanger.OpenMenu();
            foreach ((IMenu component, _) in items)
                component.OpenMenu();
        }

        public void CloseMenu()
        {
            MenuState = OpenCloseState.Closing;
            visibilityStateChanger.CloseMenu();
            foreach ((IMenu component, _) in items)
                component.CloseMenu();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;

            // Draw the backplane.
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexure,
                position: Position + glowOffset,
                color: visibilityStateChanger.Alpha * Color.White);
            spriteBatch.Draw(
                texture: backplaneTexture, 
                position: Position + backPlaneOffset, 
                color: visibilityStateChanger.Alpha * Color.White);
            spriteBatch.End();

            // Draw the other elements of the menu.
            foreach ((IMenu item, _) in items)
                item.Draw(transformMatrix: transformMatrix);
        }
        
        public void Update(GameTime gameTime)
        {
            // For every component, set its position and runs its update function.
            foreach ((IMenu component, Vector2 itemOffset) in items)
            {
                component.Position = Position + itemOffset;
                component.Update(gameTime);
            }

            // If the controller is set, the up and down buttons can be used to move through each controllable component.
            if (Controller != null)
            {
                if (Controller.DownPressed())
                {
                    int indexWithController = items.FindIndex((tuple) => tuple.Item1.Controller == Controller);
                    if (indexWithController >= 0)
                    {
                        items[indexWithController].Item1.Controller = null;
                        for (int index = 0; index < items.Count; index++)
                        {
                            int nextIndex = (indexWithController + 1 + index) % items.Count;
                            items[nextIndex].Item1.Controller = Controller;
                            if (items[nextIndex].Item1.Controller == Controller)
                                break;
                        }
                    }
                }
                if (Controller.UpPressed())
                {
                    int indexWithController = items.FindIndex((item) => item.Item1.Controller == Controller);
                    if (indexWithController >= 0)
                    {
                        items[indexWithController].Item1.Controller = null;
                        for (int index = 0; index < items.Count; index++)
                        {
                            int prevIndex = ((indexWithController - (1 + index)) % items.Count + items.Count) % items.Count;
                            items[prevIndex].Item1.Controller = Controller;
                            if (items[prevIndex].Item1.Controller == Controller)
                                break;
                        }
                    }
                }
            }

            // Update state.
            if (MenuState == OpenCloseState.Opening && items.Select((item) => item.Item1.MenuState).All((x) => x == OpenCloseState.Opened))
                MenuState = OpenCloseState.Opened;
            if (MenuState == OpenCloseState.Closing && items.Select((item) => item.Item1.MenuState).All((x) => x == OpenCloseState.Closed))
                MenuState = OpenCloseState.Closed;
            visibilityStateChanger.Update(gameTime);
        }

        public void SoftReset()
        {
            visibilityStateChanger.SoftReset();
            foreach ((IMenu component, _) in items)
                component.SoftReset();
        }

        public void HardReset()
        {
            visibilityStateChanger.HardReset();
            foreach ((IMenu component, _) in items)
                component.HardReset();
        }
    }
}
