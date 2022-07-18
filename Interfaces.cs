using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal enum Alignment { Center, Left, Right };
    internal interface IMenu : IComponent, IControllable, IAppliable, IMovable, ISizable
    {
        Alignment Align { get; set; }
    }
    internal interface IComponent : IDrawable, IUpdateable
    {
    }
    internal interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null);
    }
    internal interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
    internal interface IDestroyable : IDisposable
    {
        bool Destroyed { get; }
    }
    internal interface ISemiPausible
    {
        bool SemiPaused { get; }
        void SemiPause();
        void SemiResume();
    }
    internal interface IFullPausible
    {
        bool FullPaused { get; }
        void FullPause();
        void FullResume();
    }
    internal interface ISavable<T> where T : struct
    {
        T Save();
        void Load(T save);
    }
    internal interface IController : IUpdateable, IDefaultable
    {
        float LeftHeld();
        float RightHeld();
        float UpHeld();
        float DownHeld();
        bool LeftPressed();
        bool RightPressed();
        bool UpPressed();
        bool DownPressed();
        bool ActivatePressed();
        bool CollectKeys { get; set; }
        List<TextInputEventArgs> KeysPressed { get; }
    }
    internal interface IDefaultable
    {
        void ApplyDefaults();
    }
    internal interface IAppliable
    {
        void ApplyChanges();
    }
    internal interface IControllable
    {
        IController Controller { get; set; }
    }
}
