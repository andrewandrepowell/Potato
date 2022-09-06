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
        private ContainerMenu loadContainerMenu;
        private ContainerMenu saveContainerMenu;
        private ContainerMenu placeWallContainerMenu;
        private TypingMenu loadTypingMenu;
        private TypingMenu saveTypingMenu;
        private SelectMenu titleSelectMenu;
        private SelectMenu loadSelectMenu;
        private SelectMenu saveSelectMenu;
        private SelectMenu placeWallSelectMenu;
        private SelectMenu hideSelectMenu;
        private string wallToPlaceIdentifier;
        private List<(SelectMenu, string)> wallToPlaceItems;
        private IController hideController;
        public string WallToPlaceIdentifier => wallToPlaceIdentifier;
        public StringBuilder LoadString => loadTypingMenu.Text;
        public bool LoadMenuActive => loadContainerMenu.Controller != null;
        public StringBuilder SaveString => saveTypingMenu.Text;
        public bool SaveMenuActive => saveContainerMenu.Controller != null;
        public ISelectable TitleSelect => titleSelectMenu;
        public IOpenable.OpenStates OpenState => transitionMenu.OpenState;
        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }

        public LevelEditorMenu()
        {
            Rectangle gameBounds = Potato.Game.GraphicsDevice.Viewport.Bounds;

            hideController = null;

            wallToPlaceIdentifier = "";
            wallToPlaceItems = new List<(SelectMenu, string)>();
            foreach (string text in WallManager.Identifiers)
                wallToPlaceItems.Add((new SelectMenu(text: text, align: Alignment.Center, width: innerWidth), text));

            saveTypingMenu = new TypingMenu(width: innerWidth);
            loadTypingMenu = new TypingMenu(width: innerWidth);

            titleSelectMenu = new SelectMenu(text: "Title Menu", align: Alignment.Center, width: innerWidth);
            loadSelectMenu = new SelectMenu(text: "Load Level", align: Alignment.Center, width: innerWidth);
            saveSelectMenu = new SelectMenu(text: "Save Level", align: Alignment.Center, width: innerWidth);
            placeWallSelectMenu = new SelectMenu(text: "Place Wall", align: Alignment.Center, width: innerWidth);
            hideSelectMenu = new SelectMenu(text: "Hide Menu", align: Alignment.Center, width: innerWidth);

            mainContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    titleSelectMenu,
                    loadSelectMenu,
                    saveSelectMenu,
                    placeWallSelectMenu,
                    hideSelectMenu
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


            loadContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "Load Name:", align: Alignment.Center, width: innerWidth),
                    loadTypingMenu
                },
                align: Alignment.Center);
            loadContainerMenu.Position = new Vector2(
                x: (gameBounds.Width - loadContainerMenu.Size.Width) / 2,
                y: (gameBounds.Height - loadContainerMenu.Size.Height) / 2);

            saveContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "Save Name:", align: Alignment.Center, width: innerWidth),
                    saveTypingMenu
                },
                align: Alignment.Center);
            saveContainerMenu.Position = new Vector2(
                x: (gameBounds.Width - saveContainerMenu.Size.Width) / 2,
                y: (gameBounds.Height - saveContainerMenu.Size.Height) / 2);

            TransitionMenu.Node mainContainerNode = new TransitionMenu.Node(
                selectable: null,
                menu: mainContainerMenu);
            TransitionMenu.Node placeWallContainerNode = new TransitionMenu.Node(
                selectable: placeWallSelectMenu,
                menu: placeWallContainerMenu);
            TransitionMenu.Node loadContainerNode = new TransitionMenu.Node(
                selectable: loadSelectMenu,
                menu: loadContainerMenu);
            TransitionMenu.Node saveContainerNode = new TransitionMenu.Node(
                selectable: saveSelectMenu,
                menu: saveContainerMenu);

            mainContainerNode.Nodes.Add(placeWallContainerNode);
            mainContainerNode.Nodes.Add(loadContainerNode);
            mainContainerNode.Nodes.Add(saveContainerNode);

            transitionMenu = new TransitionMenu(
                nodes: mainContainerNode.Nodes,
                menu: mainContainerMenu);
        }
        public void Close() => transitionMenu.Close();

        public void Draw(Matrix? transformMatrix = null) => transitionMenu.Draw(transformMatrix: transformMatrix);

        private void Reset()
        {
            wallToPlaceIdentifier = "";
        }

        public void HardReset()
        {
            Reset();
            transitionMenu.HardReset();
        }

        public void Open() => transitionMenu.Open();

        public void SoftReset()
        {
            Reset();
            transitionMenu.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            // Update the wall to place if selected.
            foreach ((SelectMenu selectMenu, string text) in wallToPlaceItems)
                if (selectMenu.Selected)
                    wallToPlaceIdentifier = text;

            // If in the place wall menu, user can go back if the back button is pressed.
            if (placeWallContainerMenu.Controller != null && placeWallContainerMenu.Controller.BackPressed())
                transitionMenu.GoPreviousMenu();

            // There are two conditions to go back to the previous menu if the save container menu has the controller.
            //  1) If the back is pressed and the text field has no characters.
            //  2) OR, if the activate pressed and the text field has characters.
            if (saveContainerMenu.Controller != null && 
                ((saveContainerMenu.Controller.BackPressed() && saveTypingMenu.Text.Length == 0) ||
                (saveContainerMenu.Controller.ActivatePressed() && saveTypingMenu.Text.Length != 0)))
                transitionMenu.GoPreviousMenu();

            // The load container menu works the same way as the save menu.
            if (loadContainerMenu.Controller != null &&
                ((loadContainerMenu.Controller.BackPressed() && loadTypingMenu.Text.Length == 0) ||
                (loadContainerMenu.Controller.ActivatePressed() && loadTypingMenu.Text.Length != 0)))
                transitionMenu.GoPreviousMenu();

            // Close the menu and remove its control if the hide menu option is selected.
            if (hideSelectMenu.Selected && hideController == null)
            {
                hideController = transitionMenu.Controller;
                transitionMenu.Controller = null;
                transitionMenu.Close();
            }

            // Return control back to and open the menu if the back is pressed.
            if (hideController != null && hideController.BackPressed())
            {
                transitionMenu.Controller = hideController;
                hideController = null;
                transitionMenu.Open();
            }

            // Update transition menu
            transitionMenu.Update(gameTime: gameTime);
        }
    }
}
