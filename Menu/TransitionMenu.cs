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
            public ContainerMenu Container { get; private set; }
            public ICollection<Node> Nodes { get; private set; }
            
            public Node(ISelectable selectable, ContainerMenu container)
            {
                Selectable = selectable;
                Container = container;
                Nodes = new List<Node>();
            }
        }
        private enum TransitionState { Idle, Transitioning, Reversing };
        private Stack<(ICollection<Node>, ContainerMenu)> stack;
        private TransitionState transitionState;
        private Node nextNode;
        private ICollection<Node> currentNodes => stack.Peek().Item1;
        private ContainerMenu currentMenu => stack.Peek().Item2;
        private bool backEnable;
        public MenuState State => currentMenu.State;
        public IController Controller { get => currentMenu.Controller; set => currentMenu.Controller = value; }
        public Vector2 Position { get => currentMenu.Position; set => currentMenu.Position = value; }
        public Size2 Size { get => currentMenu.Size; set => currentMenu.Size = value; }

        public TransitionMenu(ICollection<Node> nodes, ContainerMenu container, bool backEnable)
        {
            stack = new Stack<(ICollection<Node>, ContainerMenu)>();
            stack.Push((nodes, container));
            transitionState = TransitionState.Idle;
            this.backEnable = backEnable;
        }

        public void CloseMenu() => currentMenu.CloseMenu();

        public void OpenMenu() => currentMenu.OpenMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null) =>
            currentMenu.Draw(spriteBatch: spriteBatch, transformMatrix: transformMatrix);

        public void Update(GameTime gameTime)
        {            
            switch (transitionState)
            {
                case TransitionState.Idle:
                    foreach (Node node in currentNodes)
                    {
                        if (node.Selectable.Selected)
                        {
                            nextNode = node;
                            currentMenu.CloseMenu();
                            transitionState = TransitionState.Transitioning;
                        }
                    }
                    if (backEnable && currentMenu.Controller.BackPressed() && stack.Count > 1)
                    {
                        currentMenu.CloseMenu();
                        transitionState = TransitionState.Reversing;
                    }
                    break;
                case TransitionState.Transitioning:
                    if (currentMenu.State == MenuState.Closed)
                    {
                        foreach (Node node in currentNodes)
                            node.Selectable.Selected = false;
                        nextNode.Container.Controller = currentMenu.Controller;
                        currentMenu.Controller = null;
                        nextNode.Container.Position = new Vector2(
                            x: currentMenu.Position.X + (currentMenu.Size.Width - nextNode.Container.Size.Width) / 2,
                            y: currentMenu.Position.Y + (currentMenu.Size.Height - nextNode.Container.Size.Height) / 2);
                        nextNode.Container.OpenMenu();
                        stack.Push((nextNode.Nodes, nextNode.Container));
                        nextNode = null;
                        transitionState = TransitionState.Idle;
                    }
                    break;
                case TransitionState.Reversing:
                    if (currentMenu.State == MenuState.Closed)
                    {
                        foreach (Node node in currentNodes)
                            node.Selectable.Selected = false;
                        ContainerMenu previousMenu = currentMenu;
                        stack.Pop();
                        currentMenu.Controller = previousMenu.Controller;
                        previousMenu.Controller = null;
                        currentMenu.Position = new Vector2(
                            x: previousMenu.Position.X + (previousMenu.Size.Width - currentMenu.Size.Width) / 2,
                            y: previousMenu.Position.Y + (previousMenu.Size.Height - currentMenu.Size.Height) / 2);
                        currentMenu.OpenMenu();
                        transitionState = TransitionState.Idle;
                    }
                    break;
            }

            currentMenu.Update(gameTime: gameTime);
        }
    }
}
