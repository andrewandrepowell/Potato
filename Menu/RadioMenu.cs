using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Diagnostics;

namespace Potato.Menu
{
    internal class RadioMenu : IMenu
    {
        private static SpriteFont font;
        private static readonly Color fontColor = Potato.ColorTheme0;
        private static SpriteSheet radioSpriteSheet;
        private const float spaceBetweenOptions = 10f;
        private readonly List<(AnimatedSprite, string, Vector2, Vector2)> items = new List<(AnimatedSprite, string, Vector2, Vector2)>();
        private const float alphaChangeRate = 1.0f;
        private bool alphaIncrement = false;
        private float alpha = 1.0f;
        public List<string> Options { get; private set; } = new List<string>();
        public int Selected { get; set; } = 0;
        public Alignment Align { get; set; } = Alignment.Left;
        public IController Controller { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Size2 Size { get; set; } = Size2.Empty;

        public RadioMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
            if (radioSpriteSheet == null)
                radioSpriteSheet = Potato.Game.Content.Load<SpriteSheet>("radio.sf", new JsonContentLoader());
        }

        public void ApplyChanges()
        {
            if (Size.Width < 0)
                throw new ArgumentOutOfRangeException();
            items.Clear();
            List<(AnimatedSprite, string, Vector2, Vector2)> line = new List<(AnimatedSprite, string, Vector2, Vector2)>();
            Size2 radioSize = radioSpriteSheet.TextureAtlas.First().Size;
            float height = Math.Max(radioSize.Height, font.MeasureString(" ").Y);
            float widthOffset = 0;
            float heightOffset = 0;
            foreach (var option in Options.Select((x)=>x.Trim()).Detailed())
            {
                if (widthOffset + spaceBetweenOptions + radioSize.Width + font.MeasureString(option.Value).X > Size.Width)
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
                    foreach ((AnimatedSprite radioSprite, string optionValue, Vector2 radioOffset, Vector2 optionOffset) in line)
                    {
                        items.Add((
                            radioSprite,
                            optionValue,
                            new Vector2(x: radioOffset.X + lineWidthOffset, y: radioOffset.Y),
                            new Vector2(x: optionOffset.X + lineWidthOffset, y: optionOffset.Y)));
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
                        line.Add((
                            radioSprite, 
                            option.Value, 
                            new Vector2(x: radioWidthOffset, y:radioHeightOffset), 
                            new Vector2(x: optionWidthOffset, y:optionHeightOffset)));
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
                        foreach ((AnimatedSprite radioSprite, string optionValue, Vector2 radioOffset, Vector2 optionOffset) in line)
                        {
                            items.Add((
                                radioSprite,
                                optionValue,
                                new Vector2(x: radioOffset.X + lineWidthOffset, y: radioOffset.Y),
                                new Vector2(x: optionOffset.X + lineWidthOffset, y: optionOffset.Y)));
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
                        spriteSheet: radioSpriteSheet);
                    line.Add((
                        radioSprite,
                        option.Value,
                        new Vector2(x: radioWidthOffset, y: radioHeightOffset),
                        new Vector2(x: optionWidthOffset, y: optionHeightOffset)));
                }
            }
            Size = new Size2(
                width: Size.Width,
                height: heightOffset);
            ApplySelected();
        }

        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix);
            foreach ((int index, var tuple) in items.Select((tuple, index) => (index, tuple)))
            {
                AnimatedSprite radioSprite = tuple.Item1;
                string optionValue = tuple.Item2;
                Vector2 radioOffset = tuple.Item3;
                Vector2 optionOffset = tuple.Item4;
                spriteBatch.Draw(
                    sprite: radioSprite,
                    position: Position + radioOffset);
                spriteBatch.DrawString(
                    spriteFont: font,
                    text: optionValue,
                    position: Position + optionOffset,
                    color: fontColor * ((index == Selected) ? alpha : 1.0f));
            }
            spriteBatch.End();
        }
        
        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Controller != null)
            {
                if (Controller.RightPressed())
                {
                    Selected = (Selected == items.Count - 1) ? 0 : Selected + 1;
                    ApplySelected();
                }
                if (Controller.LeftPressed())
                {
                    Selected = (Selected == 0) ? items.Count - 1 : Selected - 1;
                    ApplySelected();
                }
            }

            if (Controller != null)
            {
                alpha += (alphaIncrement ? 1.0f : -1.0f) * alphaChangeRate * timeElapsed;
                if (alpha > 1.0f)
                    alphaIncrement = false;
                else if (alpha < 0.0f)
                    alphaIncrement = true;
            }
            else
            {
                alpha = 1.0f;
                alphaIncrement = false;
            }

            foreach (var tuple in items)
            {
                AnimatedSprite radioSprite = tuple.Item1;
                radioSprite.Update(timeElapsed);
            }
        }

        private void ApplySelected()
        {
            foreach ((int index, var tuple) in items.Select((x, index) => (index, x)))
            {
                AnimatedSprite radioSprite = tuple.Item1;
                if (index == Selected)
                    radioSprite.Play("selected");
                else
                    radioSprite.Play("unselected");
            }
        }
    }
}
