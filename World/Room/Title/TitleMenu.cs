using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;

namespace Potato.World.Room.Title
{
    internal class TitleMenu : IMenu
    {
        public OpenCloseState MenuState => throw new NotImplementedException();
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Size2 Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CloseMenu()
        {
            throw new NotImplementedException();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            throw new NotImplementedException();
        }

        public void OpenMenu()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
