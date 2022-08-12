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
        private EngineEditorMenu engineEditorMenu;
        public ISelectable TitleSelect => engineEditorMenu.TitleSelect;
        public OpenCloseState RoomState => roomStateChanger.RoomState;
        public IController Controller { get => engineEditorMenu.Controller; set => engineEditorMenu.Controller = value; }
        
        public EngineEditorRoom(OptionMenu optionMenu)
        {
            roomStateChanger = new RoomStateChanger();
            engineEditorMenu = new EngineEditorMenu(optionMenu: optionMenu);
            engineEditorMenu.Position = new Vector2(
                x: (Potato.Game.GraphicsDevice.Viewport.Width - engineEditorMenu.Size.Width) / 2,
                y: (Potato.Game.GraphicsDevice.Viewport.Height - engineEditorMenu.Size.Height) / 2);
            engineEditorMenu.OpenMenu();
            Controller = null;
        }

        public void CloseRoom() => roomStateChanger.CloseRoom();

        public void Draw(Matrix? transformMatrix = null)
        {
            Potato.SpriteBatch.GraphicsDevice.Clear(Color.RosyBrown);
            engineEditorMenu.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void HardReset()
        {
            engineEditorMenu.HardReset();
            roomStateChanger.HardReset();
        }

        public void OpenRoom() => roomStateChanger.OpenRoom();

        public void SoftReset()
        {
            engineEditorMenu.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            engineEditorMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
