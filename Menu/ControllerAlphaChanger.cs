using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class ControllerAlphaChanger : IUpdateable
    {
        private const float changeRate = 1.0f;
        private bool increment = false;
        private IControllable controllable;
        public float Alpha { get; private set; } = 0.0f;

        public ControllerAlphaChanger(IControllable controllable)
        {
            this.controllable = controllable;
        }
        
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (controllable.Controller != null)
            {
                Alpha += (increment ? 1.0f : -1.0f) * changeRate * timeElapsed;
                if (Alpha > 1.0f)
                    increment = false;
                else if (Alpha < 0.0f)
                    increment = true;
            }
            else
            {
                Alpha = 1.0f;
                increment = false;
            }
        }
    }
}
