using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Text;

namespace Potato
{
    internal struct KeyboardControllerSave
    {
        public Keys ActivateKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
        public KeyboardControllerSave(Keys activateKey, Keys leftKey, Keys rightKey, Keys upKey, Keys downKey)
        {
            ActivateKey = activateKey;
            LeftKey = leftKey;
            RightKey = rightKey;
            UpKey = upKey;
            DownKey = downKey;
        }
    }
    internal class KeyboardController : IController, IDisposable, ISavable<KeyboardControllerSave>
    {
        private KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
        public StringBuilder Text { get ; private set; } = new StringBuilder();
        public bool CollectText { get; set; } = false;
        public KeyboardController()
        {
            Potato.Game.Window.TextInput += ServiceTextInput;
            ApplyDefaults();
        }
        public Keys ActivateKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
        public KeyboardControllerSave Save() => new KeyboardControllerSave(ActivateKey, LeftKey, RightKey, UpKey, DownKey);

        public void Load(KeyboardControllerSave save)
        {
            ActivateKey = save.ActivateKey;
            LeftKey = save.LeftKey;
            RightKey = save.RightKey;
            UpKey = save.UpKey;
            DownKey = save.DownKey;
        }
        public void ApplyDefaults()
        {
            ActivateKey = Keys.Enter;
            LeftKey = Keys.Left;
            RightKey = Keys.Right;
            UpKey = Keys.Up;
            DownKey = Keys.Down;
        }
        public bool ActivatePressed() => keyboardState.WasKeyJustDown(ActivateKey);
        public float DownHeld() => (DownPressed()) ? 1.0f : 0.0f;

        public bool DownPressed() => keyboardState.WasKeyJustDown(DownKey);

        public float LeftHeld() => (LeftPressed()) ? 1.0f : 0.0f;

        public bool LeftPressed() => keyboardState.WasKeyJustDown(LeftKey);

        public float RightHeld() => (RightPressed()) ? 1.0f : 0.0f;

        public bool RightPressed() => keyboardState.WasKeyJustDown(RightKey);

        public float UpHeld() => (UpPressed()) ? 1.0f : 0.0f;

        public bool UpPressed() => keyboardState.WasKeyJustDown(UpKey);
        
        public void Update(GameTime gameTime) => keyboardState = KeyboardExtended.GetState();
        
        public void ServiceTextInput(object sender, TextInputEventArgs e)
        {
            if (CollectText)
                Text.Append(e.Character);
        }

        public void Dispose() => Potato.Game.Window.TextInput -= ServiceTextInput;
    }
}
