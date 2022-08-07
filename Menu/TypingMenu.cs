﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Menu
{
    internal class TypingMenu : IMenu
    {
        private static SpriteFont font = null;
        private Texture2D fieldTexture;
        private static readonly Color fieldColor = Potato.ColorTheme0;
        private Texture2D glowTexture;
        private Vector2 glowOffset;
        private IController controller;
        private int cursor;
        private const string cursorString = "|";
        private Vector2 cursorOffset;
        private const float cursorVisibleTimerThreshold = 0.5f;
        private float cursorVisibleTimer;
        private bool cursorVisible;
        private TrackKey cursorTrackLeft;
        private TrackKey cursorTrackRight;
        private Vector2 textOffset;
        private static readonly Color textColor = Potato.ColorTheme3;
        private float fieldHeight, fieldWidth;
        private Size2 size;
        private VisibilityStateChanger visibilityStateChanger;

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
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public StringBuilder Text { get; private set; }
        public OpenCloseState MenuState { get => visibilityStateChanger.State; }

        public TypingMenu(float width)
        {
            Debug.Assert(width > 0);
            if (font == null)
                font = Potato.Game.Content.Load<SpriteFont>("font");
            float textHeight = font.MeasureString(" ").Y;
            fieldWidth = width;
            fieldHeight = textHeight + 4;
            size = new Size2(width: fieldWidth + 8, height: fieldHeight + 8);
            textOffset = new Vector2(x: 0, y: (fieldHeight - textHeight) / 2);

            fieldTexture = new Texture2D(
                graphicsDevice: Potato.SpriteBatch.GraphicsDevice,
                width: (int)fieldWidth,
                height: (int)fieldHeight,
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

            Text = new StringBuilder("");
            cursor = 0;
            controller = null;
            cursorOffset = Vector2.Zero;
            cursorVisibleTimer = cursorVisibleTimerThreshold;
            cursorVisible = false;
            visibilityStateChanger = new VisibilityStateChanger();
            cursorTrackLeft = new TrackKey();
            cursorTrackRight = new TrackKey();
        }

        public void OpenMenu() => visibilityStateChanger.OpenMenu();

        public void CloseMenu() => visibilityStateChanger.CloseMenu();

        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: glowTexture,
                position: Position + glowOffset,
                color: visibilityStateChanger.Alpha * Color.White);
            spriteBatch.Draw(
                texture: fieldTexture,
                position: Position,
                color: visibilityStateChanger.Alpha * Color.White);
            spriteBatch.DrawString(
                spriteFont: font, 
                text: Text, 
                position: Position + textOffset, 
                color: visibilityStateChanger.Alpha * textColor);
            spriteBatch.DrawString(
                spriteFont: font,
                text: cursorString,
                position: Position + cursorOffset,
                color: visibilityStateChanger.Alpha * ((cursorVisible) ? 1.0f : 0.0f) * textColor);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                        else if (font.MeasureString(Text.ToString() + e.Character).X <= fieldWidth)
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
                if (cursorTrackLeft.Check() && cursor > 0)
                    cursor--;
                if (cursorTrackRight.Check() && cursor < Text.Length)
                    cursor++;

                // Update cursor position.
                Vector2 textSize = font.MeasureString((cursor > 0) ? Text.ToString().Substring(0, cursor) : " ");
                cursorOffset = new Vector2(
                    x: (cursor > 0) ? (textSize.X - 4) : 0,
                    y: (Size.Height - textSize.Y) / 2 - 7);

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

            // Update state
            visibilityStateChanger.Update(gameTime);
        }

        public void SoftReset()
        {
            visibilityStateChanger.SoftReset();
        }

        public void HardReset()
        {
            visibilityStateChanger.HardReset();
            Text.Clear();
            cursor = 0;
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
