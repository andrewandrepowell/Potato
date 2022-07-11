using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal class Menu : IComponent, IDestroyable
    {
        bool IDestroyable.Destroyed => throw new NotImplementedException();

        void IDestroyable.Destroy()
        {
            throw new NotImplementedException();
        }

        void IDrawable.Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        void IUpdateable.Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
