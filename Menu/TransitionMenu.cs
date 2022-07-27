using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class TransitionMenu : IMenu
    {
        public class Node
        {
            public ISelectable Selectable { get; private set; }
            public IMenu Menu { get; private set; }
            public ICollection<Node> Nodes { get; private set; }
            
            public Node(ISelectable selectable, IMenu menu)
            {
                Selectable = selectable;
                Menu = menu;
                Nodes = new List<Node>();
            }
        }
        private enum TransitionState { Idle, Transitioning, Reversing };
        private Stack<(ICollection<Node>, IMenu)> stack;
        private TransitionState transitionState;
        private Node nextNode;
        private bool forceBack;
        public ICollection<Node> CurrentNodes => stack.Peek().Item1;
        public IMenu CurrentMenu => stack.Peek().Item2;
        
        public bool BackEnable { get; set; }
        public MenuState State => CurrentMenu.State;
        public IController Controller { get => CurrentMenu.Controller; set => CurrentMenu.Controller = value; }
        public Vector2 Position { get => CurrentMenu.Position; set => CurrentMenu.Position = value; }
        public Size2 Size { get => CurrentMenu.Size; set => CurrentMenu.Size = value; }

        public TransitionMenu(ICollection<Node> nodes, IMenu menu)
        {
            stack = new Stack<(ICollection<Node>, IMenu)>();
            stack.Push((nodes, menu));
            transitionState = TransitionState.Idle;
            forceBack = false;
            BackEnable = false;
        }

        public void CloseMenu() => CurrentMenu.CloseMenu();

        public void OpenMenu() => CurrentMenu.OpenMenu();

        public void ForceBack() => forceBack = true;

        public void Draw(Matrix? transformMatrix = null) =>
            CurrentMenu.Draw(transformMatrix: transformMatrix);

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
                            CurrentMenu.CloseMenu();
                            transitionState = TransitionState.Transitioning;
                        }
                    }
                    if (forceBack || (BackEnable && CurrentMenu.Controller.BackPressed() && stack.Count > 1))
                    {
                        forceBack = false;
                        CurrentMenu.CloseMenu();
                        transitionState = TransitionState.Reversing;
                    }
                    break;
                case TransitionState.Transitioning:
                    if (CurrentMenu.State == MenuState.Closed)
                    {
                        foreach (Node node in CurrentNodes)
                            node.Selectable.ResetMedia();
                        nextNode.Menu.Controller = CurrentMenu.Controller;
                        CurrentMenu.Controller = null;
                        nextNode.Menu.Position = new Vector2(
                            x: CurrentMenu.Position.X + (CurrentMenu.Size.Width - nextNode.Menu.Size.Width) / 2,
                            y: CurrentMenu.Position.Y + (CurrentMenu.Size.Height - nextNode.Menu.Size.Height) / 2);
                        nextNode.Menu.OpenMenu();
                        stack.Push((nextNode.Nodes, nextNode.Menu));
                        nextNode = null;
                        transitionState = TransitionState.Idle;
                    }
                    break;
                case TransitionState.Reversing:
                    if (CurrentMenu.State == MenuState.Closed)
                    {
                        foreach (Node node in CurrentNodes)
                            node.Selectable.ResetMedia();
                        IMenu previousMenu = CurrentMenu;
                        stack.Pop();
                        CurrentMenu.Controller = previousMenu.Controller;
                        previousMenu.Controller = null;
                        CurrentMenu.Position = new Vector2(
                            x: previousMenu.Position.X + (previousMenu.Size.Width - CurrentMenu.Size.Width) / 2,
                            y: previousMenu.Position.Y + (previousMenu.Size.Height - CurrentMenu.Size.Height) / 2);
                        CurrentMenu.OpenMenu();
                        transitionState = TransitionState.Idle;
                    }
                    break;
            }

            CurrentMenu.Update(gameTime: gameTime);
        }
    }
}
