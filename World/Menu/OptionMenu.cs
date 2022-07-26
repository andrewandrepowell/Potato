using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Potato.Menu;

namespace Potato.World.Menu
{
    internal enum DisplayModeType { Fullscreen, Windowed }
    internal struct OptionMenuSave
    {
        public float MasterVolume;
        public float MusicVolume;
        public float EffectVolume;
        public DisplayModeType DisplayMode;
        public Keys ActivateKey;
        public Keys BackKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
    }
    internal class OptionMenu : IMenu, ISavable<OptionMenuSave>, IDefaultable
    {
        private class ConfigureKeybindMenu : IMenu
        {
            private ContainerMenu containerMenu;
            public ConfigureKeybindMenu()
            {
                containerMenu = new ContainerMenu(
                    components: new List<IMenu>()
                    {
                        new TextMenu(text: "Hit a key to set new binding.", align: Alignment.Center, width: innerWidth),
                    },
                    align: Alignment.Center);
            }
            public CacheTextMenu SelectMenu { get; set; }
            public MenuState State => containerMenu.State;
            public IController Controller {  get => containerMenu.Controller; set => containerMenu.Controller = value; }
            public Vector2 Position { get => containerMenu.Position; set => containerMenu.Position = value; }
            public Size2 Size { get => containerMenu.Size; set => containerMenu.Size = value; }
            public void CloseMenu() => containerMenu.CloseMenu();
            public void Draw(Matrix? transformMatrix) =>
                containerMenu.Draw(transformMatrix: transformMatrix);
            public void OpenMenu() => containerMenu.OpenMenu();
            public void Update(GameTime gameTime) => containerMenu.Update(gameTime);
        }
        private static readonly Dictionary<Keys,string> keyToStringDict;
        private static readonly Dictionary<string, Keys> stringToKeyDict;
        private static readonly OptionMenuSave defaultOptionMenuSave = new OptionMenuSave()
        {
            MasterVolume = 0.5f,
            MusicVolume = 0.1f,
            EffectVolume = 1.0f,
            DisplayMode = DisplayModeType.Windowed,
            ActivateKey = Keys.Enter,
            BackKey = Keys.Back,
            LeftKey = Keys.Left,
            RightKey = Keys.Right,
            UpKey = Keys.Up,
            DownKey = Keys.Down
        };
        private const float outerWidth = 512f;
        private const float innerWidth = outerWidth * .90f;
        private const float dividerWidth = innerWidth * .90f;
        private ContainerMenu mainContainerMenu;
        private ContainerMenu keybindContainerMenu;
        private TransitionMenu transitionMenu;
        private SliderMenu masterVolumeSliderMenu;
        private SliderMenu musicVolumeSliderMenu;
        private SliderMenu effectVolumeSliderMenu;
        private RadioMenu displayModeRadioMenu;
        private SelectMenu keybindSelectMenu;
        private SelectMenu applyDefaultSelectMenu;
        private SelectMenu applyChangesSelectMenu;
        private CacheTextMenu activateKeyBindSelectMenu;
        private CacheTextMenu backKeyBindSelectMenu;
        private CacheTextMenu leftKeyBindSelectMenu;
        private CacheTextMenu rightKeyBindSelectMenu;
        private CacheTextMenu upKeyBindSelectMenu;
        private CacheTextMenu downKeyBindSelectMenu;
        private bool lockOutOfKeybindConfig;
        private bool lockOutOfApplyDefault;
        private Keys[] previousKeyPresses;
        public MenuState State => transitionMenu.State;

        public IController Controller { get => transitionMenu.Controller; set => transitionMenu.Controller = value; }
        public KeyboardController Keyboard { get; set; }
        public Vector2 Position { get => transitionMenu.Position; set => transitionMenu.Position = value; }
        public Size2 Size { get => transitionMenu.Size; set => transitionMenu.Size = value; }

        public OptionMenu(OptionMenuSave save) : this()
        {
            Load(save: save);
        }
        
        public OptionMenu()
        {
            OptionMenuSave save = defaultOptionMenuSave;
            masterVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: save.MasterVolume);
            musicVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: save.MusicVolume);
            effectVolumeSliderMenu = new SliderMenu(width: innerWidth, fill: save.EffectVolume);
            keybindSelectMenu = new SelectMenu(text: "Configure Key Bindings", align: Alignment.Center, width: innerWidth);
            displayModeRadioMenu = new RadioMenu(
                options: new List<string>()
                {
                    "Windowed",
                    "Fullscreen",
                },
                align: Alignment.Center,
                width: innerWidth,
                selected:
                    (save.DisplayMode == DisplayModeType.Windowed) ? 0 :
                    (save.DisplayMode == DisplayModeType.Fullscreen) ? 1 :
                    throw new ArgumentException());
            applyDefaultSelectMenu = new SelectMenu(text: "Apply Defaults", align: Alignment.Center, width: innerWidth);
            applyChangesSelectMenu = new SelectMenu(text: "Apply Changes", align: Alignment.Center, width: innerWidth);
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
                    new DividerMenu(width: dividerWidth),
                    applyDefaultSelectMenu,
                    applyChangesSelectMenu,
                },
                align: Alignment.Center);
            activateKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Activate: {tuple.Key}").ToList(),
                align: Alignment.Center, width: innerWidth)
            {
                Text = $"Activate: {keyToStringDict[save.ActivateKey]}"
            };
            backKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Back: {tuple.Key}").ToList(),
                align: Alignment.Center, width: innerWidth)
            {
                Text = $"Back: {keyToStringDict[save.BackKey]}"
            };
            leftKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Left: {tuple.Key}").ToList(),
                align: Alignment.Center,
                width: innerWidth)
            {
                Text = $"Left: {keyToStringDict[save.LeftKey]}"
            };
            rightKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Right: {tuple.Key}").ToList(),
                align: Alignment.Center,
                width: innerWidth)
            {
                Text = $"Right: {keyToStringDict[save.RightKey]}"
            };
            upKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Up: {tuple.Key}").ToList(),
                align: Alignment.Center,
                width: innerWidth)
            {
                Text = $"Up: {keyToStringDict[save.UpKey]}"
            };
            downKeyBindSelectMenu = new CacheTextMenu(
                texts: stringToKeyDict.Select((tuple) => $"Down: {tuple.Key}").ToList(),
                align: Alignment.Center,
                width: innerWidth)
            {
                Text = $"Down: {keyToStringDict[save.DownKey]}"
            };
            keybindContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "-Key Bindings-", align: Alignment.Center, width: outerWidth),
                    new DividerMenu(width: dividerWidth),
                    activateKeyBindSelectMenu,
                    backKeyBindSelectMenu,
                    leftKeyBindSelectMenu,
                    rightKeyBindSelectMenu,
                    upKeyBindSelectMenu,
                    downKeyBindSelectMenu,
                },
                align: Alignment.Center);
            ContainerMenu configureKeybindContainerMenu = new ContainerMenu(
                components: new List<IMenu>()
                {
                    new TextMenu(text: "Hit a key to set new binding.", align: Alignment.Center, width: innerWidth),
                },
                align: Alignment.Center);

            var activateKeybindNode = new TransitionMenu.Node(selectable: activateKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = activateKeyBindSelectMenu });
            var backKeybindNode = new TransitionMenu.Node(selectable: backKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = backKeyBindSelectMenu });
            var leftKeybindNode = new TransitionMenu.Node(selectable: leftKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = leftKeyBindSelectMenu });
            var rightKeybindNode = new TransitionMenu.Node(selectable: rightKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = rightKeyBindSelectMenu });
            var upKeybindNode = new TransitionMenu.Node(selectable: upKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = upKeyBindSelectMenu });
            var downKeybindNode = new TransitionMenu.Node(selectable: downKeyBindSelectMenu, menu: new ConfigureKeybindMenu() { SelectMenu = downKeyBindSelectMenu });
            var keybindNode = new TransitionMenu.Node(selectable: keybindSelectMenu, menu: keybindContainerMenu);
            keybindNode.Nodes.Add(activateKeybindNode);
            keybindNode.Nodes.Add(backKeybindNode);
            keybindNode.Nodes.Add(leftKeybindNode);
            keybindNode.Nodes.Add(rightKeybindNode);
            keybindNode.Nodes.Add(upKeybindNode);
            keybindNode.Nodes.Add(downKeybindNode);
            transitionMenu = new TransitionMenu(nodes: new List<TransitionMenu.Node>() { keybindNode }, menu: mainContainerMenu) { BackEnable = true };
            lockOutOfKeybindConfig = false;
            lockOutOfApplyDefault = false;
            previousKeyPresses = null;
        }
        
        public void CloseMenu() => transitionMenu.CloseMenu();

        public void Draw(Matrix? transformMatrix = null) =>
            transitionMenu.Draw(transformMatrix: transformMatrix);

        public void OpenMenu() => transitionMenu.OpenMenu();

        public void Update(GameTime gameTime)
        {
            ConfigureKeybindMenu configureKeybindMenu = transitionMenu.CurrentMenu as ConfigureKeybindMenu;
            if (configureKeybindMenu != null)
            {
                if (!lockOutOfKeybindConfig)
                {
                    KeyboardStateExtended keyboardState = Keyboard.KeyboardState;
                    Keys[] pressedKeys = keyboardState.GetPressedKeys();
                    transitionMenu.BackEnable = false;
                    if (previousKeyPresses != null)
                    {
                        foreach (Keys key in previousKeyPresses)
                        {
                            string keyString = keyToStringDict[key];
                            if (keyboardState.WasKeyJustDown(key))
                            {
                                string[] tokens = configureKeybindMenu.SelectMenu.Text.Split(' ');
                                configureKeybindMenu.SelectMenu.Text = $"{tokens[0]} {keyString}";
                                transitionMenu.BackEnable = true;
                                transitionMenu.ForceBack();
                                lockOutOfKeybindConfig = true;
                                break;
                            }
                        }
                    }
                    previousKeyPresses = pressedKeys;
                }
            }
            else
            {
                transitionMenu.BackEnable = true;
                lockOutOfKeybindConfig = false;
                previousKeyPresses = null;
            }

            if (applyDefaultSelectMenu.Selected)
            {
                if (!lockOutOfApplyDefault)
                {
                    lockOutOfApplyDefault = true;
                    ApplyDefaults();
                }
            }
            else lockOutOfApplyDefault = false;

            transitionMenu.Update(gameTime: gameTime);
        }

        public OptionMenuSave Save() => new OptionMenuSave()
        {
            MasterVolume = masterVolumeSliderMenu.Fill,
            MusicVolume = musicVolumeSliderMenu.Fill,
            EffectVolume = effectVolumeSliderMenu.Fill,
            DisplayMode =
                (displayModeRadioMenu.Selected == 0) ? DisplayModeType.Fullscreen :
                (displayModeRadioMenu.Selected == 1) ? DisplayModeType.Windowed :
                throw new ArgumentException(),
            ActivateKey = stringToKeyDict[activateKeyBindSelectMenu.Text.Split(' ')[1]],
            BackKey = stringToKeyDict[backKeyBindSelectMenu.Text.Split(' ')[1]],
            LeftKey = stringToKeyDict[leftKeyBindSelectMenu.Text.Split(' ')[1]],
            RightKey = stringToKeyDict[rightKeyBindSelectMenu.Text.Split(' ')[1]],
            UpKey = stringToKeyDict[upKeyBindSelectMenu.Text.Split(' ')[1]],
            DownKey = stringToKeyDict[downKeyBindSelectMenu.Text.Split(' ')[1]]
        };

        public void Load(OptionMenuSave save)
        {
            masterVolumeSliderMenu.Fill = save.MasterVolume;
            musicVolumeSliderMenu.Fill = save.MusicVolume;
            effectVolumeSliderMenu.Fill = save.EffectVolume;
            displayModeRadioMenu.Selected = 
                (save.DisplayMode == DisplayModeType.Windowed) ? 0 :
                (save.DisplayMode == DisplayModeType.Fullscreen) ? 1 :
                throw new ArgumentException();
            activateKeyBindSelectMenu.Text = $"Activate: {keyToStringDict[save.ActivateKey]}";
            backKeyBindSelectMenu.Text = $"Back: {keyToStringDict[save.BackKey]}";
            leftKeyBindSelectMenu.Text = $"Left: {keyToStringDict[save.LeftKey]}";
            rightKeyBindSelectMenu.Text = $"Right: {keyToStringDict[save.RightKey]}";
            upKeyBindSelectMenu.Text = $"Up: {keyToStringDict[save.UpKey]}";
            downKeyBindSelectMenu.Text = $"Down: {keyToStringDict[save.DownKey]}";
        }

        public void ApplyDefaults()
        {
            Load(save: defaultOptionMenuSave);
        }

        static OptionMenu()
        {
            keyToStringDict = new Dictionary<Keys, string>();
            keyToStringDict.Add(key: Keys.Back, value: "Backspace");
            keyToStringDict.Add(key: Keys.Tab, value: "Tab");
            keyToStringDict.Add(key: Keys.Enter, value: "Enter");
            keyToStringDict.Add(key: Keys.Pause, value: "Pause");
            keyToStringDict.Add(key: Keys.CapsLock, value: "Caps Lock");
            keyToStringDict.Add(key: Keys.Escape, value: "Escape");
            keyToStringDict.Add(key: Keys.Space, value: "Space");
            keyToStringDict.Add(key: Keys.PageUp, value: "Page Up");
            keyToStringDict.Add(key: Keys.PageDown, value: "Page Down");
            keyToStringDict.Add(key: Keys.End, value: "End");
            keyToStringDict.Add(key: Keys.Home, value: "Home");
            keyToStringDict.Add(key: Keys.Left, value: "Left");
            keyToStringDict.Add(key: Keys.Up, value: "Up");
            keyToStringDict.Add(key: Keys.Right, value: "Right");
            keyToStringDict.Add(key: Keys.Down, value: "Down");
            keyToStringDict.Add(key: Keys.Insert, value: "Insert");
            keyToStringDict.Add(key: Keys.Delete, value: "Delete");
            keyToStringDict.Add(key: Keys.D0, value: "0");
            keyToStringDict.Add(key: Keys.D1, value: "1");
            keyToStringDict.Add(key: Keys.D2, value: "2");
            keyToStringDict.Add(key: Keys.D3, value: "3");
            keyToStringDict.Add(key: Keys.D4, value: "4");
            keyToStringDict.Add(key: Keys.D5, value: "5");
            keyToStringDict.Add(key: Keys.D6, value: "6");
            keyToStringDict.Add(key: Keys.D7, value: "7");
            keyToStringDict.Add(key: Keys.D8, value: "8");
            keyToStringDict.Add(key: Keys.D9, value: "9");
            keyToStringDict.Add(key: Keys.A, value: "A");
            keyToStringDict.Add(key: Keys.B, value: "B");
            keyToStringDict.Add(key: Keys.C, value: "C");
            keyToStringDict.Add(key: Keys.D, value: "D");
            keyToStringDict.Add(key: Keys.E, value: "E");
            keyToStringDict.Add(key: Keys.F, value: "F");
            keyToStringDict.Add(key: Keys.G, value: "G");
            keyToStringDict.Add(key: Keys.H, value: "H");
            keyToStringDict.Add(key: Keys.I, value: "I");
            keyToStringDict.Add(key: Keys.J, value: "J");
            keyToStringDict.Add(key: Keys.K, value: "K");
            keyToStringDict.Add(key: Keys.L, value: "L");
            keyToStringDict.Add(key: Keys.M, value: "M");
            keyToStringDict.Add(key: Keys.N, value: "N");
            keyToStringDict.Add(key: Keys.O, value: "O");
            keyToStringDict.Add(key: Keys.P, value: "P");
            keyToStringDict.Add(key: Keys.Q, value: "Q");
            keyToStringDict.Add(key: Keys.R, value: "R");
            keyToStringDict.Add(key: Keys.S, value: "S");
            keyToStringDict.Add(key: Keys.T, value: "T");
            keyToStringDict.Add(key: Keys.U, value: "U");
            keyToStringDict.Add(key: Keys.V, value: "V");
            keyToStringDict.Add(key: Keys.W, value: "W");
            keyToStringDict.Add(key: Keys.X, value: "X");
            keyToStringDict.Add(key: Keys.Y, value: "Y");
            keyToStringDict.Add(key: Keys.Z, value: "Z");
            keyToStringDict.Add(key: Keys.NumPad0, value: "NumPad 0");
            keyToStringDict.Add(key: Keys.NumPad1, value: "NumPad 1");
            keyToStringDict.Add(key: Keys.NumPad2, value: "NumPad 2");
            keyToStringDict.Add(key: Keys.NumPad3, value: "NumPad 3");
            keyToStringDict.Add(key: Keys.NumPad4, value: "NumPad 4");
            keyToStringDict.Add(key: Keys.NumPad5, value: "NumPad 5");
            keyToStringDict.Add(key: Keys.NumPad6, value: "NumPad 6");
            keyToStringDict.Add(key: Keys.NumPad7, value: "NumPad 7");
            keyToStringDict.Add(key: Keys.NumPad8, value: "NumPad 8");
            keyToStringDict.Add(key: Keys.NumPad9, value: "NumPad 9");
            keyToStringDict.Add(key: Keys.Multiply, value: "Multiply");
            keyToStringDict.Add(key: Keys.Add, value: "Add");
            keyToStringDict.Add(key: Keys.Separator, value: "Separator");
            keyToStringDict.Add(key: Keys.Subtract, value: "Subtract");
            keyToStringDict.Add(key: Keys.Decimal, value: "Decimal");
            keyToStringDict.Add(key: Keys.Divide, value: "Divide");
            keyToStringDict.Add(key: Keys.F1, value: "F1");
            keyToStringDict.Add(key: Keys.F2, value: "F2");
            keyToStringDict.Add(key: Keys.F3, value: "F3");
            keyToStringDict.Add(key: Keys.F4, value: "F4");
            keyToStringDict.Add(key: Keys.F5, value: "F5");
            keyToStringDict.Add(key: Keys.F6, value: "F6");
            keyToStringDict.Add(key: Keys.F7, value: "F7");
            keyToStringDict.Add(key: Keys.F8, value: "F8");
            keyToStringDict.Add(key: Keys.F9, value: "F9");
            keyToStringDict.Add(key: Keys.F10, value: "F10");
            keyToStringDict.Add(key: Keys.F11, value: "F11");
            keyToStringDict.Add(key: Keys.F12, value: "F12");
            keyToStringDict.Add(key: Keys.F13, value: "F13");
            keyToStringDict.Add(key: Keys.F14, value: "F14");
            keyToStringDict.Add(key: Keys.F15, value: "F15");
            keyToStringDict.Add(key: Keys.F16, value: "F16");
            keyToStringDict.Add(key: Keys.F17, value: "F17");
            keyToStringDict.Add(key: Keys.F18, value: "F18");
            keyToStringDict.Add(key: Keys.F19, value: "F19");
            keyToStringDict.Add(key: Keys.F20, value: "F20");
            keyToStringDict.Add(key: Keys.F21, value: "F21");
            keyToStringDict.Add(key: Keys.F22, value: "F22");
            keyToStringDict.Add(key: Keys.F23, value: "F23");
            keyToStringDict.Add(key: Keys.F24, value: "F24");
            keyToStringDict.Add(key: Keys.NumLock, value: "NumLock");
            keyToStringDict.Add(key: Keys.Scroll, value: "Scroll");
            keyToStringDict.Add(key: Keys.LeftShift, value: "LeftShift");
            keyToStringDict.Add(key: Keys.RightShift, value: "RightShift");
            keyToStringDict.Add(key: Keys.LeftControl, value: "LeftControl");
            keyToStringDict.Add(key: Keys.RightControl, value: "RightControl");
            keyToStringDict.Add(key: Keys.LeftAlt, value: "LeftAlt");
            keyToStringDict.Add(key: Keys.RightAlt, value: "RightAlt");
            keyToStringDict.Add(key: Keys.BrowserBack, value: "BrowserBack");
            keyToStringDict.Add(key: Keys.BrowserForward, value: "BrowserForward");
            keyToStringDict.Add(key: Keys.BrowserRefresh, value: "BrowserRefresh");
            keyToStringDict.Add(key: Keys.BrowserStop, value: "BrowserStop");
            keyToStringDict.Add(key: Keys.BrowserSearch, value: "BrowserSearch");
            keyToStringDict.Add(key: Keys.BrowserFavorites, value: "BrowserFavorites");
            keyToStringDict.Add(key: Keys.BrowserHome, value: "BrowserHome");
            keyToStringDict.Add(key: Keys.VolumeMute, value: "VolumeMute");
            keyToStringDict.Add(key: Keys.VolumeDown, value: "VolumeDown");
            keyToStringDict.Add(key: Keys.VolumeUp, value: "VolumeUp");
            keyToStringDict.Add(key: Keys.MediaNextTrack, value: "MediaNextTrack");
            keyToStringDict.Add(key: Keys.MediaPreviousTrack, value: "MediaPreviousTrack");
            keyToStringDict.Add(key: Keys.MediaStop, value: "MediaStop");
            keyToStringDict.Add(key: Keys.MediaPlayPause, value: "MediaPlayPause");
            keyToStringDict.Add(key: Keys.LaunchMail, value: "LaunchMail");
            keyToStringDict.Add(key: Keys.SelectMedia, value: "SelectMedia");
            keyToStringDict.Add(key: Keys.LaunchApplication1, value: "LaunchApplication1");
            keyToStringDict.Add(key: Keys.LaunchApplication2, value: "LaunchApplication2");
            keyToStringDict.Add(key: Keys.OemSemicolon, value: "OemSemicolon");
            keyToStringDict.Add(key: Keys.OemPlus, value: "OemPlus");
            keyToStringDict.Add(key: Keys.OemComma, value: "OemComma");
            keyToStringDict.Add(key: Keys.OemMinus, value: "OemMinus");
            keyToStringDict.Add(key: Keys.OemPeriod, value: "OemPeriod");
            keyToStringDict.Add(key: Keys.OemQuestion, value: "OemQuestion");
            keyToStringDict.Add(key: Keys.OemTilde, value: "OemTilde");
            keyToStringDict.Add(key: Keys.OemOpenBrackets, value: "OemOpenBrackets");
            keyToStringDict.Add(key: Keys.OemPipe, value: "OemPipe");
            keyToStringDict.Add(key: Keys.OemCloseBrackets, value: "OemCloseBrackets");
            keyToStringDict.Add(key: Keys.OemQuotes, value: "OemQuotes");
            keyToStringDict.Add(key: Keys.Oem8, value: "Oem8");
            keyToStringDict.Add(key: Keys.OemBackslash, value: "OemBackslash");
            keyToStringDict.Add(key: Keys.ProcessKey, value: "ProcessKey");
            keyToStringDict.Add(key: Keys.Attn, value: "Attn");
            keyToStringDict.Add(key: Keys.Crsel, value: "Crsel");
            keyToStringDict.Add(key: Keys.Exsel, value: "Exsel");
            keyToStringDict.Add(key: Keys.EraseEof, value: "EraseEof");
            keyToStringDict.Add(key: Keys.Play, value: "Play");
            keyToStringDict.Add(key: Keys.Zoom, value: "Zoom");
            keyToStringDict.Add(key: Keys.Pa1, value: "Pa1");
            keyToStringDict.Add(key: Keys.OemClear, value: "OemClear");
            stringToKeyDict = keyToStringDict.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }
    }
}
