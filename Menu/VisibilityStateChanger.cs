using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class VisibilityStateChanger : IUpdateable
    {
        public MenuState State { get; private set; } = MenuState.Closed;
        public float Alpha { get; private set; } = 0.0f;
        private const float stateAlphaChangeRate = 1.0f;
        
        public void OpenMenu()
        {
            Alpha = 0.0f;
            State = MenuState.Opening;
        }

        public void CloseMenu()
        {
            Alpha = 1.0f;
            State = MenuState.Closing;
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (State)
            {
                case MenuState.Closed:
                    Alpha = 0.0f;
                    break;
                case MenuState.Opened:
                    Alpha = 1.0f;
                    break;
                case MenuState.Opening:
                    Alpha += stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha > 1.0f)
                    {
                        Alpha = 1.0f;
                        State = MenuState.Opened;
                    }
                    break;
                case MenuState.Closing:
                    Alpha -= stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha < 0.0f)
                    {
                        Alpha = 0.0f;
                        State = MenuState.Closed;
                    }
                    break;
            }
        }
    }
}
