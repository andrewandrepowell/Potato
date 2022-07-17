using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.GlowEffect;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal static class Texture2DExtensions
    {
        public static Texture2D CreateStandardGlow0(this Texture2D texture) =>
            GlowEffect.CreateGlow(
                src: texture,
                color: Potato.ColorTheme3,
                size: 2,
                strength: 0.6f,
                alphaIncreaser: 0.5f,
                alphaReducer: 0.15f,
                alphaDistance: 0.2f,
                graphics: Potato.Game.GraphicsDevice,
                glowType: GlowType.GlowWithoutTexture);
    }
}
