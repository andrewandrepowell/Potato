using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Room
{
    internal class TransitionRoom : IRoom
    {
        public class Node
        {
            public ISelectable Selectable { get; private set; }
            public IRoom Room { get; private set; }
            public ICollection<Node> Nodes { get; private set; }

            public Node(ISelectable selectable, IRoom room)
            {
                Selectable = selectable;
                Room = room;
                Nodes = new List<Node>();
            }
        }
        private enum TransitionState { Idle, Transitioning, Reversing };
        private TransitionState transitionState;
        private Node nextNode;
        public ICollection<Node> CurrentNodes { get; private set;  }
        public IRoom CurrentRoom { get; private set; }

        public OpenCloseState RoomState => CurrentRoom.RoomState;

        public IController Controller { get => CurrentRoom.Controller; set => CurrentRoom.Controller = value; }

        public TransitionRoom(ICollection<Node> nodes, IRoom room)
        {
            CurrentNodes = nodes;
            CurrentRoom = room;
            transitionState = TransitionState.Idle;
        }

        public void CloseRoom() => CurrentRoom.CloseRoom();

        public void Draw(Matrix? transformMatrix = null) => CurrentRoom.Draw(transformMatrix: transformMatrix);

        public void OpenRoom() => CurrentRoom.OpenRoom();

        public void Update(GameTime gameTime)
        {
            switch (transitionState)
            {
                case TransitionState.Idle:
                    foreach (Node node in CurrentNodes)
                    {
                        if (node.Selectable.Selected)
                        {
                            nextNode = node;
                            CurrentRoom.CloseRoom();
                            transitionState = TransitionState.Transitioning;
                        }
                    }
                    break;
                case TransitionState.Transitioning:
                    if (CurrentRoom.RoomState == OpenCloseState.Closed)
                    {
                        CurrentRoom.SoftReset();
                        nextNode.Room.Controller = CurrentRoom.Controller;
                        CurrentRoom.Controller = null;
                        CurrentRoom = nextNode.Room;
                        CurrentNodes = nextNode.Nodes;
                        CurrentRoom.OpenRoom();
                        nextNode = null;
                        transitionState = TransitionState.Idle;
                    }
                    break;
            }

            CurrentRoom.Update(gameTime: gameTime);
        }

        public void SoftReset() => CurrentRoom.SoftReset();

        public void HardReset() => CurrentRoom.HardReset();
    }
}
