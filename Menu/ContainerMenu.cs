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
        }
    }
}
