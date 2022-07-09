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
}
