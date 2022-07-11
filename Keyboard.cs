using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

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
    internal class KeyboardController : IController, IDefaultable, ISavable<KeyboardControllerSave>
    {
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
        public bool ActivatePressed() => KeyboardExtended.GetState().WasKeyJustDown(ActivateKey);
        public float DownHeld() => (DownPressed()) ? 1.0f : 0.0f;

        public bool DownPressed() => KeyboardExtended.GetState().WasKeyJustDown(DownKey);

        public float LeftHeld() => (LeftPressed()) ? 1.0f : 0.0f;

        public bool LeftPressed() => KeyboardExtended.GetState().WasKeyJustDown(LeftKey);

        public float RightHeld() => (RightPressed()) ? 1.0f : 0.0f;

        public bool RightPressed() => KeyboardExtended.GetState().WasKeyJustDown(RightKey);

        public float UpHeld() => (UpPressed()) ? 1.0f : 0.0f;

        public bool UpPressed() => KeyboardExtended.GetState().WasKeyJustDown(UpKey);
    }
}
