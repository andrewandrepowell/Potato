using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.Menu
{
    internal class SelectImageMenu : IMenu, ISelectable
    {
        private Texture2D texture;
        private Size2 size;
        private readonly ControllerAlphaChanger controllerAlphaChanger;
        private readonly VisibilityStateChanger state;
        private readonly SelectChanger selectChanger;
        public IController Controller { get => controllerAlphaChanger.Controller; set => controllerAlphaChanger.Controller = value; }
        public Vector2 Position { get; set; }
        public Size2 Size { get => size; set { throw new NotImplementedException(); } }
        public bool Selected { get => selectChanger.Selected; }
        public MenuState State { get => state.State; }

        public SelectImageMenu(Texture2D texture)
        {
            this.texture = texture;
            size = new Size2(texture.Width, texture.Height);
            controllerAlphaChanger = new ControllerAlphaChanger();
            state = new VisibilityStateChanger();
            selectChanger = new SelectChanger();
            Controller = null;
            Position = Vector2.Zero;
        }

        public void OpenMenu() => state.OpenMenu();

        public void CloseMenu() => state.CloseMenu();
        
        public void Draw(Matrix? transformMatrix = null)
        {
            SpriteBatch spriteBatch = Potato.SpriteBatch;
            spriteBatch.Begin(transformMatrix: transformMatrix);
            spriteBatch.Draw(
                texture: texture, 
                position: Position, 
                color: state.Alpha * controllerAlphaChanger.Alpha * selectChanger.ApplySelect(Color.White));
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (Controller != null && Controller.ActivatePressed())
                selectChanger.Select();

            state.Update(gameTime);
            controllerAlphaChanger.Update(gameTime);
            selectChanger.Update(gameTime);
        }

        private static Color Add(Color color1, Color color2) => new Color(
           color1.R + color2.R,
           color1.G + color2.G,
           color1.B + color2.B,
           color1.A + color2.A);

        public void Select() => selectChanger.Select();

        public void ResetMedia() => selectChanger.ResetMedia();
    }
}
