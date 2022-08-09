using Microsoft.Xna.Framework;
using Potato.Room;
using Potato.World.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class EngineEditorRoom : IRoom
    {
        private RoomStateChanger roomStateChanger;
        
        public OpenCloseState RoomState => roomStateChanger.RoomState;

        public IController Controller { get; set; }
        
        public EngineEditorRoom(OptionMenu optionMenu)
        {
            roomStateChanger = new RoomStateChanger();
            Controller = null;
        }

        public void CloseRoom() => roomStateChanger.CloseRoom();

        public void Draw(Matrix? transformMatrix = null)
        {
            Potato.SpriteBatch.GraphicsDevice.Clear(Color.RosyBrown);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void HardReset()
        {
            roomStateChanger.HardReset();
        }

        public void OpenRoom() => roomStateChanger.OpenRoom();

        public void SoftReset() => roomStateChanger.SoftReset();

        public void Update(GameTime gameTime)
        {
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
