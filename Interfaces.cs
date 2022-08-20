using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal interface IOpenable
    {
        enum OpenStates { Opened, Opening, Closed, Closing };
        OpenStates OpenState { get; }
        void Open();
        void Close();
    }
    internal interface ICollidable : IMovable, IDestroyable
    {
        struct Info
        {
            public ICollidable Other { get; private set; }
            public Vector2 Point { get; private set; }
            public Vector2 Correction { get; private set; }
            public Vector2 Normal { get; private set; }

            public Info(ICollidable other, Vector2 point, Vector2 correction, Vector2 normal)
            {
                Other = other;
                Point = point;
                Correction = correction;
                Normal = normal;
            }
        }
        bool Collidable { get; set; }
        Texture2D CollisionMask { get; }
        IList<Vector2> CollisionVertices { get; }
        void ServiceCollision(Info info);
    }
    internal interface IRoom : IComponent, IControllable, IResetable, IOpenable
    {
    }
    internal enum Alignment { Center, Left, Right };
    internal interface IMenu : IComponent, IControllable, IMovable, ISizable, IResetable, IOpenable
    {
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
