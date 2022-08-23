using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Potato.Menu;
using Potato.World.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class EngineEditorMenu : IMenu
    {
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private TransitionMenu transitionMenu;
        private ContainerMenu containerMenu;
        private OptionMenu optionMenu;
        private SelectMenu titleSelectMenu;
        private SelectMenu optionSelectMenu;
        private SelectMenu collisionTest0SelectMenu;
        private SelectMenu physicsTest0SelectMenu;
        public ISelectable TitleSelect => titleSelectMenu;
        public ISelectable CollisionTest0Select => collisionTest0SelectMenu;
        public ISelectable PhysicsTest0Select => physicsTest0SelectMenu;
        public IOpenable.OpenStates OpenState => transitionMenu.OpenState;
        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }

        public EngineEditorMenu(OptionMenu optionMenu)
        {
            this.optionMenu = optionMenu;
            titleSelectMenu = new SelectMenu(text: "Title Screen", align: Alignment.Center, width: innerWidth);
            optionSelectMenu = new SelectMenu(text: "Options", align: Alignment.Center, width: innerWidth);
            collisionTest0SelectMenu = new SelectMenu(text: "Collision Test 0", align: Alignment.Center, width: innerWidth);
            physicsTest0SelectMenu = new SelectMenu(text: "Physics Test 0", align: Alignment.Center, width: innerWidth);
            containerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    titleSelectMenu,
                    optionSelectMenu,
                    collisionTest0SelectMenu,
                    physicsTest0SelectMenu
                },
                align: Alignment.Center);

            transitionMenu = new TransitionMenu(
                nodes: new List<TransitionMenu.Node>()
                {
                    new TransitionMenu.Node(selectable: optionSelectMenu, menu: optionMenu)
                },
                menu: containerMenu);
        }
        public void Close() => transitionMenu.Close();

        public void Draw(Matrix? transformMatrix = null) => transitionMenu.Draw(transformMatrix: transformMatrix);

        public void HardReset()
        {
            transitionMenu.HardReset();
        }

        public void Open() => transitionMenu.Open();

        public void SoftReset()
        {
            transitionMenu.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            if (optionMenu.Controller != null && optionMenu.Controller.BackPressed() && optionMenu.IsMainMenuActive())
            {
                transitionMenu.GoPreviousMenu();
                if (transitionMenu.CurrentMenu == optionMenu)
                    Saver.Save(fileName: Potato.OptionSaveFileName, obj: optionMenu.Save());
            }

            if (optionMenu.ApplyDefaultSelected)
                Saver.Save(fileName: Potato.OptionSaveFileName, obj: optionMenu.Save());

            transitionMenu.Update(gameTime: gameTime);
        }
    }
}
