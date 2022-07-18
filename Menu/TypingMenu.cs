using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class TypingMenu : IMenu
    {
        private static SpriteFont font = null;
        private bool applyChanges = false;
        private Texture2D fieldTexture = null;
        private static readonly Color fieldColor = Potato.ColorTheme0;
        private Texture2D glowTexture = null;
        private Vector2 glowOffset = Vector2.Zero;
        private IController controller = null;
        private int cursor = 0;
        private Vector2 cursorOffset = Vector2.Zero;
        private Vector2 textOffset = Vector2.Zero;
        private static readonly Color textColor = Potato.ColorTheme3;
        public Alignment Align { get; set; } = Alignment.Left;
        public IController Controller 
        { 
            get => controller; 
            set
            {
                if (value == null)
                {
                    if (controller != null)
                        controller.CollectKeys = false;
                }
                else
                {
                    controller = value;
                    controller.CollectKeys = true;
                }
            }
        }
        public Vector2 Position { get; set; }
        public Size2 Size { get; set; }
        public StringBuilder Text { get; private set; } = new StringBuilder("");

        public TypingMenu()
        {
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
        }

        public void ApplyChanges()
        {
            float textHeight = font.MeasureString(" ").Y;
            textOffset = new Vector2(x: 0, y: (Size.Height - textHeight) / 2);
            applyChanges = true;
        }
        
        public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
        {
            if (fieldTexture == null || applyChanges)
            {
                fieldTexture = new Texture2D(
                    graphicsDevice: spriteBatch.GraphicsDevice,
                    width: (int)Size.Width,
                    height: (int)Size.Height,
                    mipmap: false,
                    format: SurfaceFormat.Color);
                fieldTexture.SetData(Enumerable
                    .Range(0, fieldTexture.Width * fieldTexture.Height)
                    .Select((x) => fieldColor)
                    .ToArray());
                glowTexture = fieldTexture.CreateStandardGlow0();
                glowOffset = new Vector2(
                    x: -(glowTexture.Width - fieldTexture.Width) / 2,
                    y: -(glowTexture.Height - fieldTexture.Height) / 2);
            }
            applyChanges = false;

            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexture,
                position: Position + glowOffset,
                color: Color.White);
            spriteBatch.Draw(
                texture: fieldTexture,
                position: Position,
                color: Color.White);
            spriteBatch.DrawString(
                spriteFont: font, 
                text: Text, 
                position: Position + textOffset, 
                color: textColor);
            spriteBatch.DrawString(
                spriteFont: font,
                text: "|",
                position: Position + cursorOffset,
                color: textColor);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (Controller != null)
            {
                List<TextInputEventArgs> keysPressed = Controller.KeysPressed;
                if (keysPressed.Count > 0)
                {
                    foreach (TextInputEventArgs e in keysPressed)
                    {
                        if (e.Character == '\b')
                        {
                            if (cursor > 0)
                            {
                                cursor--;
                                Text.Remove(cursor, 1);
                            }
                        }
                        else if (e.Character == '\r')
                        {
                        }
                        else
                        {
                            Text.Insert(cursor, e.Character);
                            cursor++;
                        }
                    }
                    keysPressed.Clear();

                    Vector2 textSize = font.MeasureString((cursor > 0) ? Text.ToString().Substring(0, cursor) : " ");
                    cursorOffset = new Vector2(
                        x: (cursor > 0) ? textSize.X : 0, 
                        y: (Size.Height - textSize.Y) / 2);
                }
            }
        }
    }
}
