using Microsoft.Xna.Framework;
using Potato.Room;
using Potato.World.Menu;
using Potato.World.Room.EngineEditor;
using Potato.World.Room.Title;
using System;
using System.Linq;
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
            
            TransitionRoom.Node titleNode = new TransitionRoom.Node(
                selectable: new SelectCombiner(selectables: new List<ISelectable>() { engineEditorRoom.TitleSelect }, option: SelectCombiner.Options.Any),
                room: titleRoom);
            TransitionRoom.Node engineEditorNode = new TransitionRoom.Node(
                selectable: titleRoom.EngineEditorSelect,
                room: engineEditorRoom);

            titleNode.Nodes.Add(engineEditorNode);
            engineEditorNode.Nodes.Add(titleNode);

            transitionRoom = new TransitionRoom(
                nodes: titleNode.Nodes, 
                room: titleNode.Room);
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
