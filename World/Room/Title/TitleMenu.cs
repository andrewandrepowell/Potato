using System;
using System.Linq;
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
        private Vector2 containerOffset;
        private Vector2 optionOffset;
        private Vector2 titlePosition;
        private Size2 titleSize;
        public OpenCloseState MenuState => transitionMenu.MenuState;
        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position 
        { 
            get => titlePosition; 
            set
            {
                titlePosition = value;
                containerMenu.Position = value + containerOffset;
                optionMenu.Position = value + optionOffset;
            }
        }
        public Size2 Size { get => titleSize; set => throw new NotImplementedException(); }
        
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

            List<IMenu> containerMenus = new List<IMenu>() { containerMenu, optionMenu };
            titleSize = new Size2(width: containerMenus.Select((x) => x.Size.Width).Max(), height: containerMenus.Select((x) => x.Size.Height).Max());
            containerOffset = new Vector2(x: (Size.Width - containerMenu.Size.Width) / 2, y: (Size.Height - containerMenu.Size.Height) / 2);
            optionOffset = new Vector2(x: (Size.Width - optionMenu.Size.Width) / 2, y: (Size.Height - optionMenu.Size.Height) / 2);
            Position = Vector2.Zero;
        }

        public void CloseMenu() => transitionMenu.CloseMenu();

        public void Draw(Matrix? transformMatrix = null) => transitionMenu.Draw(transformMatrix: transformMatrix);

        public void OpenMenu() => transitionMenu.OpenMenu();

        public void Update(GameTime gameTime)
        {

            
            transitionMenu.Update(gameTime: gameTime);
        }
    }
}
