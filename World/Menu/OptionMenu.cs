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
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private const float defaultSliderFill = 0.5f;
        private ContainerMenu mainContainerMenu;
        private ContainerMenu keybindContainerMenu;
        private TransitionMenu transitionMenu;
        private SliderMenu masterVolumeSliderMenu;
        private SliderMenu musicVolumeSliderMenu;
        private SliderMenu effectVolumeSliderMenu;
        private RadioMenu displayModeRadioMenu;
        private SelectMenu keybindSelectMenu;
        public MenuState State => transitionMenu.State;

        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }

        public OptionMenu()
        {
            masterVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            musicVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            effectVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: defaultSliderFill);
            keybindSelectMenu = new SelectMenu(text: "Configure Key Bindings", align: Alignment.Center, width: innerWidth);
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
                    keybindSelectMenu,
                }, 
                align: Alignment.Center);
            keybindContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "-Key Bindings-", align: Alignment.Center, width: outerWidth),
                    new DividerMenu(width: dividerWidth),
                },
                align: Alignment.Center);

            var mainNodes = new List<TransitionMenu.Node>();
            mainNodes.Add(new TransitionMenu.Node(selectable: keybindSelectMenu, container: keybindContainerMenu));
            transitionMenu = new TransitionMenu(nodes: mainNodes, container: mainContainerMenu, backEnable: true);
        }
        public void CloseMenu() => transitionMenu.CloseMenu();

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null) =>
            transitionMenu.Draw(spriteBatch: spriteBatch, transformMatrix: transformMatrix);

        public void OpenMenu() => transitionMenu.OpenMenu();

        public void Update(GameTime gameTime) =>
            transitionMenu.Update(gameTime: gameTime);
    }
}
