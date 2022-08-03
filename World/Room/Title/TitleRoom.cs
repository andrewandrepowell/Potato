using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.Title
{
    internal class TitleRoom : IRoom
    {
        public IController Controller { get; set; }

        public OpenCloseState RoomState => throw new NotImplementedException();

        public void CloseRoom()
        {
            throw new NotImplementedException();
        }

        public void Draw(Matrix? transformMatrix)
        {
            throw new NotImplementedException();
        }

        public void OpenRoom()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
