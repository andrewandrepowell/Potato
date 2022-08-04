using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;

namespace Potato.World.Room.Title
{
    internal class TitleMenu : IMenu
    {
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private TransitionMenu transitionMenu;
        private ContainerMenu containerMenu;
        private OptionMenu optionMenu;
        private SelectMenu engineEditorSelectMenu;
        private SelectMenu optionsSelectMenu;
        public OpenCloseState MenuState => transitionMenu.MenuState;
        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }
        
        public TitleMenu()
        {
            engineEditorSelectMenu = new SelectMenu(text: "Engine Editor", align: Alignment.Center, width: innerWidth);
            optionsSelectMenu = new SelectMenu(text: "Options", align: Alignment.Center, width: innerWidth);
            containerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    engineEditorSelectMenu,
                    optionsSelectMenu
                },
                align: Alignment.Center);
            optionMenu = new OptionMenu();
            transitionMenu = new TransitionMenu(
                nodes: new List<TransitionMenu.Node>()
                {
                    new TransitionMenu.Node(selectable: optionsSelectMenu, menu: optionMenu)
                },
                menu: containerMenu);
        }

        public void CloseMenu() => transitionMenu.CloseMenu();

        public void Draw(Matrix? transformMatrix = null) => transitionMenu.Draw(transformMatrix: transformMatrix);

        public void OpenMenu() => transitionMenu.OpenMenu();

        public void Update(GameTime gameTime) => transitionMenu.Update(gameTime: gameTime);
    }
}
