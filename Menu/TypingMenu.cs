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
        private const string cursorString = "|";
        private Vector2 cursorOffset = Vector2.Zero;
        private const float cursorVisibleTimerThreshold = 0.5f;
        private float cursorVisibleTimer = cursorVisibleTimerThreshold;
        private bool cursorVisible = true;
        private TrackKey cursorTrackLeft = new TrackKey();
        private TrackKey cursorTrackRight = new TrackKey();
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
                    {
                        controller.CollectKeys = false;
                        cursorTrackLeft.KeyHeld = null;
                        cursorTrackRight.KeyHeld = null;
                    }
                }
                else
                {
                    value.CollectKeys = true;
                    cursorTrackLeft.KeyHeld = value.LeftHeld;
                    cursorTrackRight.KeyHeld = value.RightHeld;
                }
                controller = value;
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
                text: cursorString,
                position: Position + cursorOffset,
                color: ((cursorVisible) ? 1.0f : 0.0f) * textColor);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (Controller != null)
            {
                // Previous is kept to determine if it's visibility should persist.
                int previousCursor = cursor;
                
                // Update text.
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
                        else if (font.MeasureString(Text.ToString() + e.Character).X <= Size.Width)
                        {
                            Text.Insert(cursor, e.Character);
                            cursor++;
                        }
                    }
                    keysPressed.Clear();
                }

                // Move cursor left or right.
                cursorTrackLeft.Update(gameTime);
                cursorTrackRight.Update(gameTime);
                if (cursorTrackLeft.Check())
                {
                    if (cursor > 0)
                    {
                        cursor--;
                    }
                }
                if (cursorTrackRight.Check())
                {
                    if (cursor < Text.Length)
                    {
                        cursor++;
                    }
                }

                // Update cursor position.
                Vector2 textSize = font.MeasureString((cursor > 0) ? Text.ToString().Substring(0, cursor) : " ");
                cursorOffset = new Vector2(
                    x: (cursor > 0) ? (textSize.X - 4) : 0,
                    y: (Size.Height - textSize.Y) / 2 + - 2);

                // Cursor blinking.
                if (previousCursor != cursor)
                {
                    cursorVisible = true;
                    cursorVisibleTimer = cursorVisibleTimerThreshold;
                }
                else
                {
                    cursorVisibleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cursorVisibleTimer < 0)
                    {
                        cursorVisible = !cursorVisible;
                        cursorVisibleTimer = cursorVisibleTimerThreshold;
                    }
                }
            }
            else
            {
                // Cursor isn't availble if there's no controller.
                cursorVisible = false;
            }
        }

        private class TrackKey : IUpdateable
        {
            public delegate float KeyHeldDelegate();
            public KeyHeldDelegate KeyHeld { get; set; }
            public enum State { Idle, Held, Repeat }
            private State state = State.Idle;
            private bool activate = false;
            private float holdTimer = 0.0f;
            private const float holdTimerThreshold0 = 0.5f;
            private const float holdTimerThreshold1 = 0.1f;
            public bool Check()
            {
                bool result = activate;
                activate = false;
                return result;
            }

            public void Update(GameTime gameTime)
            {   
                float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                switch (state)
                {
                    case State.Idle:
                        if (KeyHeld() == 1.0f)
                        {
                            activate = true;
                            state = State.Held;
                            holdTimer = holdTimerThreshold0;
                        }
                        break;
                    case State.Held:
                        if (KeyHeld() == 1.0f)
                        {
                            holdTimer -= timeElapsed;
                            if (holdTimer < 0.0f)
                            {
                                activate = true;
                                state = State.Repeat;
                                holdTimer = holdTimerThreshold1;
                            }
                        }
                        else
                        {
                            state = State.Idle;
                        }
                        break;
                    case State.Repeat:
                        if (KeyHeld() == 1.0f)
                        {
                            holdTimer -= timeElapsed;
                            if (holdTimer < 0.0f)
                            {
                                activate = true;
                                holdTimer = holdTimerThreshold1;
                            }
                        }
                        else
                        {
                            state = State.Idle;
                        }
                        break; 
                }
            }
        }
    }
}
