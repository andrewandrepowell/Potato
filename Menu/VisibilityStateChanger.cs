using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class VisibilityStateChanger : IUpdateable, IResetable
    {
        public OpenCloseState State { get; private set; }
        public float Alpha { get; private set; }
        private const float stateAlphaChangeRate = 3.0f;

        public VisibilityStateChanger() => HardReset();

        public void OpenMenu()
        {
            Alpha = 0.0f;
            State = OpenCloseState.Opening;
        }

        public void CloseMenu()
        {
            Alpha = 1.0f;
            State = OpenCloseState.Closing;
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (State)
            {
                case OpenCloseState.Closed:
                    Alpha = 0.0f;
                    break;
                case OpenCloseState.Opened:
                    Alpha = 1.0f;
                    break;
                case OpenCloseState.Opening:
                    Alpha += stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha > 1.0f)
                    {
                        Alpha = 1.0f;
                        State = OpenCloseState.Opened;
                    }
                    break;
                case OpenCloseState.Closing:
                    Alpha -= stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha < 0.0f)
                    {
                        Alpha = 0.0f;
                        State = OpenCloseState.Closed;
                    }
                    break;
            }
        }

        public void SoftReset()
        {
            State = OpenCloseState.Closed;
            Alpha = 0.0f;
        }

        public void HardReset() => SoftReset();
    }
}
