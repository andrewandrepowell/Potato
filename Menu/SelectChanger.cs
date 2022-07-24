using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class SelectChanger : IUpdateable
    {
        private ISelectable selectable;
        private const float selectValueChangeRate = 8.0f;
        private bool selectValueIncrement = true;
        private float selectValue = 0.0f;
        private static readonly Color selectColor = Potato.ColorTheme1;

        private static Color Add(Color color1, Color color2) => new Color(
            color1.R + color2.R,
            color1.G + color2.G,
            color1.B + color2.B,
            color1.A + color2.A);
        
        public SelectChanger(ISelectable selectable)
        {
            this.selectable = selectable;
        }

        public Color ApplySelect(Color textColor) => Add((1.0f - selectValue) * textColor, selectValue * selectColor);

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.Seconds;
            
            // If selected, flash with select color.
            if (selectable.Selected)
            {
                selectValue += (selectValueIncrement ? 1.0f : -1.0f) * selectValueChangeRate * timeElapsed;
                if (selectValue > 1.0f)
                    selectValueIncrement = false;
                else if (selectValue < 0.0f)
                    selectValueIncrement = true;
            }
            else
            {
                selectValue = 0.0f;
                selectValueIncrement = true;
            }
        }
    }
}
