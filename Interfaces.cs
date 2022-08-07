using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal interface IRoom : IComponent, IControllable, IResetable
    {
        OpenCloseState RoomState { get; }
        void OpenRoom();
        void CloseRoom();
    }
    internal enum Alignment { Center, Left, Right };
    internal enum OpenCloseState { Opening, Opened, Closing, Closed };
    internal interface IMenu : IComponent, IControllable, IMovable, ISizable, IResetable
    {
        OpenCloseState MenuState { get;  }
        void OpenMenu();
        void CloseMenu();
    }
    internal interface ISelectable
    {
        bool Selected { get; }
        void Select();
    }
    internal interface IComponent : IDrawable, IUpdateable
    {
    }
    internal interface IDrawable
    {
        void Draw(Matrix? transformMatrix = null);
    }
    internal interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
    internal interface IDestroyable : IDisposable
    {
        bool Destroyed { get; }
    }
    internal interface IPausible
    {
        bool SoftPaused { get; }
        void SoftPause();
        void SoftResume();
        bool HardPaused { get; }
        void HardPause();
        void HardResume();
    }
    internal interface IResetable
    {
        void SoftReset();
        void HardReset();
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
        bool BackPressed();
        bool CollectKeys { get; set; }
        List<TextInputEventArgs> KeysPressed { get; }
    }
    internal interface IDefaultable
    {
        void ApplyDefaults();
    }
    internal interface IControllable
    {
        IController Controller { get; set; }
    }
}
