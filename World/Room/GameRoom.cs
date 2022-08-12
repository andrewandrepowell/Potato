using Microsoft.Xna.Framework;
using Potato.Room;
using Potato.World.Menu;
using Potato.World.Room.EngineEditor;
using Potato.World.Room.Title;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room
{
    internal class GameRoom : IRoom
    {
        private TitleRoom titleRoom;
        private EngineEditorRoom engineEditorRoom;
        private TransitionRoom transitionRoom;
        public IOpenable.OpenStates OpenState => transitionRoom.OpenState;


        public IController Controller { get => transitionRoom.Controller; set => transitionRoom.Controller = value; }

        public GameRoom(OptionMenu optionMenu)
        {
            titleRoom = new TitleRoom(optionMenu: optionMenu);
            engineEditorRoom = new EngineEditorRoom(optionMenu: optionMenu);
            List<TransitionRoom.Node> nodes = new List<TransitionRoom.Node>()
            {
                new TransitionRoom.Node(selectable: titleRoom.EngineEditorSelect, room: engineEditorRoom)
            };
            nodes.Find((x) => x.Room == engineEditorRoom).Nodes.Add(new TransitionRoom.Node(selectable: engineEditorRoom.TitleSelect, room: titleRoom));
            transitionRoom = new TransitionRoom(
                nodes: nodes, 
                room: titleRoom);
            Controller = null;
        }

        public void Close() => transitionRoom.Close();

        public void Draw(Matrix? transformMatrix = null) => transitionRoom.Draw(transformMatrix: transformMatrix);

        public void HardReset() => transitionRoom.HardReset();

        public void Open() => transitionRoom.Open();

        public void SoftReset() => transitionRoom.SoftReset();

        public void Update(GameTime gameTime) => transitionRoom.Update(gameTime: gameTime);
    }
}
