using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

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
        public ISelectable EngineEditorSelect => engineEditorSelectMenu;
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
        
        public TitleMenu(OptionMenu optionMenu)
        {
            engineEditorSelectMenu = new SelectMenu(text: "Engine Editor", align: Alignment.Center, width: innerWidth);
            optionsSelectMenu = new SelectMenu(text: "Options", align: Alignment.Center, width: innerWidth);
            containerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new ImageMenu(texture: Potato.Game.Content.Load<Texture2D>("potato")),
                    engineEditorSelectMenu,
                    optionsSelectMenu
                },
                align: Alignment.Center);
            this.optionMenu = optionMenu;
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
            if (Controller != null && Controller.BackPressed() && optionMenu.IsMainMenuActive())
            {
                transitionMenu.GoPreviousMenu();
                if (transitionMenu.CurrentMenu == optionMenu)
                    Saver.Save(fileName: Potato.OptionSaveFileName, obj: optionMenu.Save());
            }

            if (optionMenu.ApplyDefaultSelected)
                Saver.Save(fileName: Potato.OptionSaveFileName, obj: optionMenu.Save());

            transitionMenu.Update(gameTime: gameTime);
        }

        public void SoftReset() => transitionMenu.SoftReset();

        public void HardReset() => transitionMenu.HardReset();
    }
}
