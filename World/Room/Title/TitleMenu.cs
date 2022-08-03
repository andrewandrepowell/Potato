using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;

namespace Potato.World.Room.Title
{
    internal class TitleMenu : IMenu
    {
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private ContainerMenu containerMenu;
        private SelectMenu engineEditorSelectMenu;
        private SelectMenu optionsSelectMenu;
        public OpenCloseState MenuState => throw new NotImplementedException();
        public IController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Size2 Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
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
        }

        public void CloseMenu()
        {
            throw new NotImplementedException();
        }

        public void Draw(Matrix? transformMatrix = null)
        {
            throw new NotImplementedException();
        }

        public void OpenMenu()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
