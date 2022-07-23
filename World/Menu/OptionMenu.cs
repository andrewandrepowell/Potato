using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Potato.Menu;

namespace Potato.World.Menu
{
    internal class OptionMenu : IMenu
    {
        private MenuState transitionState;
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private const float defaultSliderFill = 0.5f;
        private ContainerMenu mainContainerMenu;
        private ContainerMenu keybindContainerMenu;
        private ContainerMenu currentContainerMenu;
        private ContainerMenu nextContainerMenu;
        private SliderMenu masterVolumeSliderMenu;
        private SliderMenu musicVolumeSliderMenu;
        private SliderMenu effectVolumeSliderMenu;
        private RadioMenu displayModeRadioMenu;
        private SelectMenu keyBindingsSelectMenu;
        public MenuState State => currentContainerMenu.State;

        public IController Controller { get => currentContainerMenu.Controller; set => currentContainerMenu.Controller = value; }
        public Vector2 Position { get => currentContainerMenu.Position; set => currentContainerMenu.Position = value; }
        public Size2 Size { get => currentContainerMenu.Size; set => currentContainerMenu.Size = value; }

        public OptionMenu()
        {
            masterVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            musicVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            effectVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            keyBindingsSelectMenu = new SelectMenu(text: "Configure Key Bindings", align: Alignment.Center, width: innerWidth);
            displayModeRadioMenu = new RadioMenu(
                options: new List<string>()
                {
                    "Windowed",
                    "Fullscreen",
                },
                align: Alignment.Center,
                width: innerWidth,
                selected: 0);
            mainContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "-Options-", align: Alignment.Center, width: outerWidth),
                    new DividerMenu(width: dividerWidth),
                    new TextMenu(text: "Master Volume:", align: Alignment.Center, width: outerWidth),
                    masterVolumeSliderMenu,
                    new TextMenu(text: "Music Volume:", align: Alignment.Center, width: outerWidth),
                    musicVolumeSliderMenu,
                    new TextMenu(text: "Effect Volume:", align: Alignment.Center, width: outerWidth),
                    effectVolumeSliderMenu,
                    new DividerMenu(width: dividerWidth),
                    new TextMenu(text: "Display Mode:", align: Alignment.Center, width: outerWidth),
                    displayModeRadioMenu,
                    new DividerMenu(width: dividerWidth),
                    keyBindingsSelectMenu,
                }, 
                align: Alignment.Center);
            keybindContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "-Key Bindings-", align: Alignment.Center, width: outerWidth),
                    new DividerMenu(width: dividerWidth),
                },
                align: Alignment.Center);
            currentContainerMenu = mainContainerMenu;
            nextContainerMenu = null;
            transitionState = MenuState.Opened;
        }
        public void CloseMenu() => currentContainerMenu.CloseMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null) =>
            currentContainerMenu.Draw(spriteBatch: spriteBatch, transformMatrix: transformMatrix);

        public void OpenMenu() => currentContainerMenu.OpenMenu();

        public void Update(GameTime gameTime)
        {
            switch (transitionState)
            { 
                case MenuState.Opened:
                    if (currentContainerMenu == mainContainerMenu)
                    {
                        if (keyBindingsSelectMenu.Selected)
                        {
                            nextContainerMenu = keybindContainerMenu;
                            currentContainerMenu.CloseMenu();
                            transitionState = MenuState.Closing;
                        }
                    }
                    break;
                case MenuState.Closing:
                    if (currentContainerMenu.State == MenuState.Closed)
                    {
                        keyBindingsSelectMenu.Selected = false;
                        
                        nextContainerMenu.Controller = currentContainerMenu.Controller;
                        nextContainerMenu.Position = new Vector2(
                            x: currentContainerMenu.Position.X + (currentContainerMenu.Size.Width - nextContainerMenu.Size.Width) / 2,
                            y: currentContainerMenu.Position.Y + (currentContainerMenu.Size.Height - nextContainerMenu.Size.Height) / 2);
                        currentContainerMenu.Controller = null;
                        currentContainerMenu = nextContainerMenu;
                        currentContainerMenu.OpenMenu();
                        transitionState = MenuState.Opened;
                    }
                    break;
            }
            
            currentContainerMenu.Update(gameTime: gameTime);
        }
    }
}
