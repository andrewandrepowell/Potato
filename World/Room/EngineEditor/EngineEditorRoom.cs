using Microsoft.Xna.Framework;
using Potato.Room;
using Potato.World.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potato.World.Room.EngineEditor
{
    internal class EngineEditorRoom : IRoom
    {
        private enum EngineEditorStates { Menu, TestCollision0, TestPhysics0 };
        private RoomStateChanger roomStateChanger;
        private EngineEditorMenu engineEditorMenu;
        private EngineEditorStates engineEditorState;
        private TestCollision0 testCollision0;
        private TestPhysics0 testPhysics0;
        private IController controller;
        public ISelectable TitleSelect => engineEditorMenu.TitleSelect;
        public IOpenable.OpenStates OpenState => roomStateChanger.OpenState;
        public IController Controller 
        { 
            get => controller;
            set
            {
                controller = value;
                engineEditorMenu.Controller = value;
            }
        }
        
        public EngineEditorRoom(OptionMenu optionMenu)
        {
            roomStateChanger = new RoomStateChanger();
            engineEditorMenu = new EngineEditorMenu(optionMenu: optionMenu);
            engineEditorMenu.Position = new Vector2(
                x: (Potato.Game.GraphicsDevice.Viewport.Width - engineEditorMenu.Size.Width) / 2,
                y: (Potato.Game.GraphicsDevice.Viewport.Height - engineEditorMenu.Size.Height) / 2);
            engineEditorState = EngineEditorStates.Menu;
            testCollision0 = new TestCollision0();
            testPhysics0 = new TestPhysics0();
            Controller = null;
        }

        public void Close()
        {
            engineEditorMenu.Close();
            roomStateChanger.Close();
        }
            
        public void Draw(Matrix? transformMatrix = null)
        {
            Potato.SpriteBatch.GraphicsDevice.Clear(Color.RosyBrown);

            switch (engineEditorState)
            {
                case EngineEditorStates.Menu:
                    break;
                case EngineEditorStates.TestCollision0:
                    testCollision0.Draw(transformMatrix: transformMatrix);
                    break;
                case EngineEditorStates.TestPhysics0:
                    testPhysics0.Draw(transformMatrix: transformMatrix);
                    break;
            }

            engineEditorMenu.Draw(transformMatrix: transformMatrix);
            roomStateChanger.Draw(transformMatrix: transformMatrix);
        }

        public void HardReset()
        {
            engineEditorState = EngineEditorStates.Menu;
            engineEditorMenu.Controller = controller;
            engineEditorMenu.HardReset();
            roomStateChanger.HardReset();
        }

        public void Open()
        {
            engineEditorState = EngineEditorStates.Menu;
            engineEditorMenu.Open();
            roomStateChanger.Open();
        }

        public void SoftReset()
        {
            engineEditorMenu.SoftReset();
            roomStateChanger.SoftReset();
        }

        public void Update(GameTime gameTime)
        {
            bool testSelected = false;
            bool checkBackPressed = false;

            switch (engineEditorState)
            {
                case EngineEditorStates.Menu:
                    if (engineEditorMenu.CollisionTest0Select.Selected)
                    {
                        engineEditorState = EngineEditorStates.TestCollision0;
                        testSelected = true;
                    }
                    if (engineEditorMenu.PhysicsTest0Select.Selected)
                    {
                        engineEditorState = EngineEditorStates.TestPhysics0;
                        testSelected = true;
                    }
                    break;
                case EngineEditorStates.TestCollision0:
                    checkBackPressed = true;
                    testCollision0.Update(gameTime: gameTime);
                    break;
                case EngineEditorStates.TestPhysics0:
                    checkBackPressed = true;
                    if (controller.ActivatePressed())
                    {
                        if (testPhysics0.HardPaused)
                            testPhysics0.HardResume();
                        else
                            testPhysics0.HardPause();
                    }
                    testPhysics0.Update(gameTime: gameTime);
                    break;
            }

            if (testSelected)
            {
                engineEditorMenu.Controller = null;
                engineEditorMenu.Close();
            }

            if (checkBackPressed && controller.BackPressed())
            {
                engineEditorState = EngineEditorStates.Menu;
                engineEditorMenu.Controller = controller;
                engineEditorMenu.Open();
            }
            
            engineEditorMenu.Update(gameTime: gameTime);
            roomStateChanger.Update(gameTime: gameTime);
        }
    }
}
