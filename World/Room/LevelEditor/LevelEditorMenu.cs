using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Potato.Room.Wall;

namespace Potato.World.Room.LevelEditor
{
    internal class LevelEditorMenu : IMenu
    {
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private TransitionMenu transitionMenu;
        private ContainerMenu mainContainerMenu;
        private ContainerMenu placeWallContainerMenu;
        private SelectMenu titleSelectMenu;
        private SelectMenu loadSelectMenu;
        private SelectMenu placeWallSelectMenu;
        private string wallToPlaceIdentifier;
        private List<(SelectMenu, string)> wallToPlaceItems;
        public string WallToPlaceIdentifier => wallToPlaceIdentifier;
        public ISelectable TitleSelect => titleSelectMenu;
        public IOpenable.OpenStates OpenState => transitionMenu.OpenState;
        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }


        public LevelEditorMenu()
        {
            Rectangle gameBounds = Potato.Game.GraphicsDevice.Viewport.Bounds;

            wallToPlaceIdentifier = "";
            wallToPlaceItems = new List<(SelectMenu, string)>();
            foreach (string text in WallManager.Identifiers)
                wallToPlaceItems.Add((new SelectMenu(text: text, align: Alignment.Center, width: innerWidth), text));

            titleSelectMenu = new SelectMenu(text: "Title Menu", align: Alignment.Center, width: innerWidth);
            loadSelectMenu = new SelectMenu(text: "Load Level", align: Alignment.Center, width: innerWidth);
            placeWallSelectMenu = new SelectMenu(text: "Place Wall", align: Alignment.Center, width: innerWidth);

            mainContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    titleSelectMenu,
                    loadSelectMenu,
                    placeWallSelectMenu
                },
                align: Alignment.Center);
            mainContainerMenu.Position = new Vector2(
                x: (gameBounds.Width - mainContainerMenu.Size.Width) / 2,
                y: (gameBounds.Height - mainContainerMenu.Size.Height) / 2);

            placeWallContainerMenu = new ContainerMenu(
                components: wallToPlaceItems.Select((x) => x.Item1 as IMenu).ToList(),
                align: Alignment.Center);
            placeWallContainerMenu.Position = new Vector2(
                x: (gameBounds.Width - placeWallContainerMenu.Size.Width) / 2,
                y: (gameBounds.Height - placeWallContainerMenu.Size.Height) / 2);

            TransitionMenu.Node mainContainerNode = new TransitionMenu.Node(
                selectable: null,
                menu: mainContainerMenu);
            TransitionMenu.Node placeWallContainerNode = new TransitionMenu.Node(
                selectable: placeWallSelectMenu,
                menu: placeWallContainerMenu);

            mainContainerNode.Nodes.Add(placeWallContainerNode);

            transitionMenu = new TransitionMenu(
                nodes: mainContainerNode.Nodes,
                menu: mainContainerMenu);
        }
        public void Close() => transitionMenu.Close();

        public void Draw(Matrix? transformMatrix = null) => transitionMenu.Draw(transformMatrix: transformMatrix);

        public void HardReset() => transitionMenu.HardReset();

        public void Open() => transitionMenu.Open();

        public void SoftReset() => transitionMenu.SoftReset();

        public void Update(GameTime gameTime)
        {
            // Update the wall to place if selected.
            foreach ((SelectMenu selectMenu, string text) in wallToPlaceItems)
                if (selectMenu.Selected)
                    wallToPlaceIdentifier = text;

            // If in the place wall menu, user can go back if the back button is pressed.
            if (placeWallContainerMenu.Controller != null && placeWallContainerMenu.Controller.BackPressed())
                transitionMenu.GoPreviousMenu();

            // Update transition menu
            transitionMenu.Update(gameTime: gameTime);
        }
    }
}
