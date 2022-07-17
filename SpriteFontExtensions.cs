using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.GlowEffect;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal static class SpriteFontExtensions
    {
        public static Texture2D CreateStandardGlow(this SpriteFont spriteFont, string text) =>
            GlowEffect.CreateGlowSpriteFont(
                spriteFont: spriteFont,
                text: text,
                textColor: Potato.ColorTheme0,
                glowColor: Potato.ColorTheme3,
                size: 2,
                scale: Vector2.One,
                strength: 0.6f,
                alphaIncreaser: 0.5f,
                alphaReducer: 0.15f,
                alphaDistance: 0.2f,
                graphics: Potato.Game.GraphicsDevice,
                glowType: GlowType.GlowWithoutTexture);
    }
}
