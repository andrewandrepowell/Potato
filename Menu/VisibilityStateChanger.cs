using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class VisibilityStateChanger : IUpdateable, IResetable, IOpenable
    {
        public IOpenable.OpenStates OpenState { get; private set; }
        public float Alpha { get; private set; }
        private const float stateAlphaChangeRate = 3.0f;

        public VisibilityStateChanger() => HardReset();

        public void Open()
        {
            Alpha = 0.0f;
            OpenState = IOpenable.OpenStates.Opening;
        }

        public void Close()
        {
            Alpha = 1.0f;
            OpenState = IOpenable.OpenStates.Closing;
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (OpenState)
            {
                case IOpenable.OpenStates.Closed:
                    Alpha = 0.0f;
                    break;
                case IOpenable.OpenStates.Opened:
                    Alpha = 1.0f;
                    break;
                case IOpenable.OpenStates.Opening:
                    Alpha += stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha > 1.0f)
                    {
                        Alpha = 1.0f;
                        OpenState = IOpenable.OpenStates.Opened;
                    }
                    break;
                case IOpenable.OpenStates.Closing:
                    Alpha -= stateAlphaChangeRate * (float)timeElapsed;
                    if (Alpha < 0.0f)
                    {
                        Alpha = 0.0f;
                        OpenState = IOpenable.OpenStates.Closed;
                    }
                    break;
            }
        }

        public void SoftReset()
        {
            OpenState = IOpenable.OpenStates.Closed;
            Alpha = 0.0f;
        }

        public void HardReset() => SoftReset();
    }
}
