﻿using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System.Diagnostics;

namespace Potato.Menu
{
    internal class RadioMenu : IMenu
    {
        private static BitmapFont font;
        private static readonly Color fontColor = Color.Black;
        private static SpriteSheet radioSpriteSheet;
        private const float spaceBetweenOptions = 10f;
        private readonly AnimatedSprite radioAnimatedSprite;
        private readonly List<(AnimatedSprite, string, float, float, float, float)> items = new List<(AnimatedSprite, string, float, float, float, float)>();
        public List<string> Options { get; private set; } = new List<string>();
        public int Selected { get; set; } = 0;
        public Alignment Align { get; set; } = Alignment.Left;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get; set; } = Size2.Empty;

        public RadioMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<BitmapFont>("montserrat-font");
            if (radioSpriteSheet == null)
                radioSpriteSheet = Potato.Game.Content.Load<SpriteSheet>("radio.sf", new JsonContentLoader());
            radioAnimatedSprite = new AnimatedSprite(
                spriteSheet: radioSpriteSheet,
                playAnimation: "unselected");
        }

        public void ApplyChanges()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            items.Clear();
            List<(AnimatedSprite, string, float, float, float, float)> line = new List<(AnimatedSprite, string, float, float, float, float)>();
            Size2 radioSize = radioAnimatedSprite.TextureRegion.Bounds.ToRectangleF().Size;
            float height = Math.Max(radioSize.Height, font.LineHeight);
            float widthOffset = 0;
            float heightOffset = 0;
            foreach (var option in Options.Select((x)=>x.Trim()).Detailed())
            {
                if (widthOffset + spaceBetweenOptions + radioSize.Width + font.MeasureString(option.Value).Width > Size.Width)
                {
                    Debug.Assert(line.Count != 0, $"Width {Size.Width} not large enough for option {option.Value}.");
                    float lineWidth = widthOffset - spaceBetweenOptions;
                    float lineWidthOffset = 0;
                    switch (Align)
                    {
                        case Alignment.Left:
                            lineWidthOffset = 0;
                            break;
                        case Alignment.Center:
                            lineWidthOffset = (Size.Width - lineWidth) / 2;
                            break;
                        case Alignment.Right:
                            lineWidthOffset = Size.Width - lineWidth;
                            break;
                    }
                    foreach ((AnimatedSprite radioSprite, string optionValue, float radioWidthOffset, float radioHeightOffset, float optionWidthOffset, float optionHeightOffset) in line)
                    {
                        items.Add((
                            radioSprite,
                            optionValue, 
                            radioWidthOffset + lineWidthOffset, 
                            radioHeightOffset, 
                            optionWidthOffset + lineWidthOffset, 
                            optionHeightOffset));
                    }
                    line.Clear();
                    widthOffset = 0;
                    heightOffset += height;
                }
                else if (option.IsLast)
                {
                    {
                        float radioWidthOffset = widthOffset + radioSize.Width / 2;
                        widthOffset += radioSize.Width;
                        float optionWidthOffset = widthOffset;
                        Size2 optionSize = font.MeasureString(option.Value);
                        widthOffset += optionSize.Width + spaceBetweenOptions;
                        float radioHeightOffset = heightOffset + Math.Max((height - radioSize.Height) / 2, 0) + radioSize.Height / 2;
                        float optionHeightOffset = heightOffset + Math.Max((height - optionSize.Height) / 2, 0);
                        AnimatedSprite radioSprite = new AnimatedSprite(
                            spriteSheet: radioSpriteSheet,
                            playAnimation: "unselected");
                        line.Add((radioSprite, option.Value, radioWidthOffset, radioHeightOffset, optionWidthOffset, optionHeightOffset));
                    }

                    {
                        float lineWidth = widthOffset - spaceBetweenOptions;
                        float lineWidthOffset = 0;
                        switch (Align)
                        {
                            case Alignment.Left:
                                lineWidthOffset = 0;
                                break;
                            case Alignment.Center:
                                lineWidthOffset = (Size.Width - lineWidth) / 2;
                                break;
                            case Alignment.Right:
                                lineWidthOffset = Size.Width - lineWidth;
                                break;
                        }
                        foreach ((AnimatedSprite radioSprite, string optionValue, float radioWidthOffset, float radioHeightOffset, float optionWidthOffset, float optionHeightOffset) in line)
                        {
                            items.Add((
                                radioSprite, optionValue,
                                radioWidthOffset + lineWidthOffset,
                                radioHeightOffset,
                                optionWidthOffset + lineWidthOffset,
                                optionHeightOffset));
                        }
                    }
                }
                else
                {
                    float radioWidthOffset = widthOffset + radioSize.Width / 2;
                    widthOffset += radioSize.Width;
                    float optionWidthOffset = widthOffset;
                    Size2 optionSize = font.MeasureString(option.Value);
                    widthOffset += optionSize.Width + spaceBetweenOptions;
                    float radioHeightOffset = heightOffset + Math.Max((height - radioSize.Height) / 2, 0) + radioSize.Height / 2;
                    float optionHeightOffset = heightOffset + Math.Max((height - optionSize.Height) / 2, 0);
                    AnimatedSprite radioSprite = new AnimatedSprite(
                        spriteSheet: radioSpriteSheet,
                        playAnimation: "unselected");
                    line.Add((radioSprite, option.Value, radioWidthOffset, radioHeightOffset, optionWidthOffset, optionHeightOffset));
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: heightOffset);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((AnimatedSprite radioSprite, string optionValue, float radioWidthOffset, float radioHeightOffset, float optionWidthOffset, float optionHeightOffset) in items)
            {
                spriteBatch.Draw(
                    sprite: radioSprite,
                    position: Position + new Vector2(radioWidthOffset, radioHeightOffset));
                spriteBatch.DrawString(
                    font: font,
                    text: optionValue,
                    position: Position + new Vector2(optionWidthOffset, optionHeightOffset),
                    color: fontColor);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach ((int index, var tuple) in items.Select((x, index) => (index, x)))
                if (index == Selected)
                    tuple.Item1.Play("selected");
                else
                    tuple.Item1.Play("unselected");
        }
    }
}
