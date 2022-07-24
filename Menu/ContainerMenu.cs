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
        private List<(IMenu, Vector2)> items;
        private IController controller = null;
        private Texture2D backplaneTexture = null;
        private static readonly Color backPlaneColor0 = Potato.ColorTheme2;
        private static readonly Color backPlaneColor1 = Potato.ColorTheme3;
        private const float backPlaneEdgeRadius = 8;
        private Vector2 backPlaneOffset = Vector2.Zero;
        private Texture2D glowTexure = null;
        private Vector2 glowOffset = Vector2.Zero;
        private Size2 size;
        private VisibilityStateChanger state = new VisibilityStateChanger();
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public MenuState State { get; private set; } = MenuState.Closed;
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
        }

        public void OpenMenu()
        {
            State = MenuState.Opening;
            state.OpenMenu();
            foreach ((IMenu component, _) in items)
            {
                component.OpenMenu();
            }
        }

        public void CloseMenu()
        {
            State = MenuState.Closing;
            state.CloseMenu();
            foreach ((IMenu component, _) in items)
            {
                component.CloseMenu();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {

            // Draw the backplane.
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexure,
                position: Position + glowOffset,
                color: state.Alpha * Color.White);
            spriteBatch.Draw(
                texture: backplaneTexture, 
                position: Position + backPlaneOffset, 
                color: state.Alpha * Color.White);
            spriteBatch.End();

            // Draw the other elements of the menu.
            foreach ((IMenu item, _) in items)
                item.Draw(spriteBatch: spriteBatch, transformMatrix: transformMatrix);
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
            if (State == MenuState.Opening && items.Select((item) => item.Item1.State).All((x) => x == MenuState.Opened))
                State = MenuState.Opened;
            if (State == MenuState.Closing && items.Select((item) => item.Item1.State).All((x) => x == MenuState.Closed))
                State = MenuState.Closed;
            state.Update(gameTime);
        }
    }
}
