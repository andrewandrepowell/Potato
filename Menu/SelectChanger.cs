using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class SelectChanger : IUpdateable, ISelectable, IResetable
    {
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement;
        private float selectValue = 0.0f;
        private bool selectNextState;
        private bool selectCurrentState;
        private static readonly Color selectColor = Potato.ColorTheme1;
        private const int cycleTotal = 4;
        private int cycleCurrent;
        public bool Selected => selectCurrentState;
        
        public SelectChanger() => HardReset();
        
        public Color ApplySelect(Color textColor) => ((1.0f - selectValue) * textColor).Add(selectValue * selectColor);

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            selectCurrentState = false;
            if (selectNextState)
            {
                selectNextState = false;
                selectCurrentState = true;
                cycleCurrent = 0;
                selectValue = 0.0f;
                selectValueIncrement = true;
            }
            
            if (cycleCurrent != cycleTotal)
            {
                selectValue += (selectValueIncrement ? 1.0f : -1.0f) * selectValueChangeRate * timeElapsed;
                if (selectValue > 1.0f)
                {
                    selectValue = 1.0f;
                    selectValueIncrement = false;
                }
                else if (selectValue < 0.0f)
                {
                    cycleCurrent++;
                    selectValue = 0.0f;
                    selectValueIncrement = true;
                }
            }
            else
            {
                selectValue = 0.0f;
                selectValueIncrement = true;
            }
        }

        public void Select() => selectNextState = true;

        public void SoftReset()
        {
            cycleCurrent = cycleTotal;
        }

        public void HardReset()
        {
            cycleCurrent = cycleTotal;
            selectNextState = false;
            selectCurrentState = false;
            selectValueIncrement = true;
        }
    }
}
