using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal struct KeyboardControllerSave
    {
        public Keys ActivateKey;
        public Keys BackKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
    }
    internal class KeyboardController : IController, IDisposable, ISavable<KeyboardControllerSave>
    {
        private KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
        public List<TextInputEventArgs> KeysPressed { get; private set; } = new List<TextInputEventArgs>();
        public bool CollectKeys { get; set; } = false;
        public KeyboardController()
        {
            Potato.Game.Window.TextInput += ServiceInput;
            ApplyDefaults();
        }
        public Keys ActivateKey;
        public Keys BackKey;
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
        public KeyboardControllerSave Save() => new KeyboardControllerSave()
        {
            ActivateKey = ActivateKey,
            BackKey = BackKey,
            LeftKey = LeftKey,
            RightKey = RightKey,
            UpKey = UpKey,
            DownKey = DownKey,
        };

        public void Load(KeyboardControllerSave save)
        {
            ActivateKey = save.ActivateKey;
            BackKey = save.BackKey;
            LeftKey = save.LeftKey;
            RightKey = save.RightKey;
            UpKey = save.UpKey;
            DownKey = save.DownKey;
        }
        public void ApplyDefaults()
        {
            ActivateKey = Keys.Enter;
            BackKey = Keys.Back;
            LeftKey = Keys.Left;
            RightKey = Keys.Right;
            UpKey = Keys.Up;
            DownKey = Keys.Down;
        }
        public bool ActivatePressed() => keyboardState.WasKeyJustDown(ActivateKey);

        public bool BackPressed() => keyboardState.WasKeyJustDown(BackKey);
        public float DownHeld() => (keyboardState.IsKeyDown(DownKey)) ? 1.0f : 0.0f;

        public bool DownPressed() => keyboardState.WasKeyJustDown(DownKey);

        public float LeftHeld() => (keyboardState.IsKeyDown(LeftKey)) ? 1.0f : 0.0f;

        public bool LeftPressed() => keyboardState.WasKeyJustDown(LeftKey);

        public float RightHeld() => (keyboardState.IsKeyDown(RightKey)) ? 1.0f : 0.0f;

        public bool RightPressed() => keyboardState.WasKeyJustDown(RightKey);

        public float UpHeld() => (keyboardState.IsKeyDown(UpKey)) ? 1.0f : 0.0f;

        public bool UpPressed() => keyboardState.WasKeyJustDown(UpKey);
        
        public void Update(GameTime gameTime) => keyboardState = KeyboardExtended.GetState();
        
        public void ServiceInput(object sender, TextInputEventArgs e)
        {
            if (CollectKeys)
                KeysPressed.Add(e);
        }

        public void Dispose() => Potato.Game.Window.TextInput -= ServiceInput;
    }
}
