using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class EngineEditorRoom : IRoom
    {
        public OpenCloseState RoomState => throw new NotImplementedException();

        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CloseRoom()
        {
            throw new NotImplementedException();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            throw new NotImplementedException();
        }

        public void HardReset()
        {
            throw new NotImplementedException();
        }

        public void OpenRoom()
        {
            throw new NotImplementedException();
        }

        public void SoftReset()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
