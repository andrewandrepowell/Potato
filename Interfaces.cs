using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal interface IComponent : IDrawable, IUpdateable
    {
    }

    internal interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
    internal interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
    internal interface IDestroyable
    {
        bool Destroyed { get; }
        void Destroy();
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
    internal interface IController
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
    }
    internal interface IDefaultable
    {
        void ApplyDefaults();
    }
}
